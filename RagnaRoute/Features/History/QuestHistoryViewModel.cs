using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using DynamicData.Binding;
using RagnaRoute.Data;
using RagnaRoute.Services;
using RagnaRoute.ViewExtenders;

namespace RagnaRoute.ViewModels;

public partial class QuestHistoryViewModel : ViewModelBase, INavigationChild
{
    public string DisplayName { get; } = "History";

    [ObservableProperty] private string? _selectedFamilyName;
    [ObservableProperty] private string? _selectedObjectiveName;
    [ObservableProperty] private int? _completionCount;

    public ReadOnlyObservableCollection<string> FamilyNames { get => _familyNames; }
    private ReadOnlyObservableCollection<string> _familyNames;

    public ReadOnlyObservableCollection<string> ObjectiveNames { get => _objectiveNames; }
    private ReadOnlyObservableCollection<string> _objectiveNames;

    public ReadOnlyObservableCollection<CompletionDto> Completions { get => _completions; }
    private ReadOnlyObservableCollection<CompletionDto> _completions;

    public SourceList<string> ProfileNameSource { get; }
    public SourceList<string> ObjectiveNameSource { get; }
    public SourceList<CompletionDto> CompletionSource { get; }

    private readonly ISchedulerProvider _scheduler;
    private readonly QuestService _questService;
    private readonly CompositeDisposable _cleanup = new();

    public QuestHistoryViewModel(ISchedulerProvider scheduler, QuestService questService)
    {
        _scheduler = scheduler;
        _questService = questService;

        var stringSorter = SortExpressionComparer<string>
            .Ascending(x => x);

        var timeSorter = SortExpressionComparer<CompletionDto>
            .Descending(x => x.CompletionTime);

        ProfileNameSource = new();
        ProfileNameSource.Connect()
            .ObserveOn(_scheduler.Background)
            .Sort(stringSorter)
            .ObserveOn(_scheduler.Visual)
            .Bind(out _familyNames)
            .Subscribe()
            .DisposeWith(_cleanup);

        ObjectiveNameSource = new();
        ObjectiveNameSource.Connect()
            .ObserveOn(_scheduler.Background)
            .Sort(stringSorter)
            .ObserveOn(_scheduler.Visual)
            .Bind(out _objectiveNames)
            .Subscribe()
            .DisposeWith(_cleanup);

        CompletionSource = new();
        CompletionSource.Connect()
            .ObserveOn(_scheduler.Background)
            .Sort(timeSorter)
            .ObserveOn(_scheduler.Visual)
            .Bind(out _completions)
            .Subscribe()
            .DisposeWith(_cleanup);

        this.WhenPropertyChanged(x => x.SelectedFamilyName)
            .Do(x =>
            {
                ObjectiveNameSource.Edit(async x =>
                {
                    if (SelectedFamilyName is null)
                        return;

                    x.Clear();
                    var result = await _questService.GetObjectives(SelectedFamilyName, true);
                    x.AddRange(result.Select(x => x.ObjectiveName));
                });

                SelectedObjectiveName = ObjectiveNameSource.Items.FirstOrDefault();
            })
            .Subscribe()
            .DisposeWith(_cleanup);

        this.WhenPropertyChanged(x => x.SelectedObjectiveName)
            .Do(x =>
            {
                CompletionSource.Edit(async x =>
                {
                    if (SelectedFamilyName is null || SelectedObjectiveName is null)
                        return;

                    x.Clear();
                    var result = await _questService.GetCompletions(SelectedFamilyName, SelectedObjectiveName);
                    x.AddRange(result);
                    CompletionCount = result.Count;
                });
            })
            .Subscribe()
            .DisposeWith(_cleanup);
    }

    public async Task InitializeProfiles()
    {
        var items = await _questService.GetFamilyNames();
        ProfileNameSource.AddRange(items);

        SelectedFamilyName = items.FirstOrDefault();
    }

    [RelayCommand]
    public async Task UpdateHistory()
    {
        if (string.IsNullOrEmpty(SelectedFamilyName) || string.IsNullOrEmpty(SelectedObjectiveName))
            return;

        var items = await _questService.GetCompletions(SelectedFamilyName, SelectedObjectiveName);
        CompletionSource.Edit(x =>
        {
            x.Clear();
            x.AddRange(items);
        });
    }
}
