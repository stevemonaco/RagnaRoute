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

    [ObservableProperty] private string? _selectedProfileName;
    [ObservableProperty] private string? _selectedObjectiveName;

    public ReadOnlyObservableCollection<string> ProfileNames { get => _profileNames; }
    private ReadOnlyObservableCollection<string> _profileNames;

    public ReadOnlyObservableCollection<string> ObjectiveNames { get => _objectiveNames; }
    private ReadOnlyObservableCollection<string> _objectiveNames;

    public ReadOnlyObservableCollection<QuestCompletionDto> Completions { get => _completions; }
    private ReadOnlyObservableCollection<QuestCompletionDto> _completions;

    public SourceList<string> ProfileNameSource { get; }
    public SourceList<string> ObjectiveNameSource { get; }
    public SourceList<QuestCompletionDto> CompletionSource { get; }

    private readonly ISchedulerProvider _scheduler;
    private readonly QuestService _questService;
    private readonly CompositeDisposable _cleanup = new();

    public QuestHistoryViewModel(ISchedulerProvider scheduler, QuestService questService)
    {
        _scheduler = scheduler;
        _questService = questService;

        var stringSorter = SortExpressionComparer<string>
            .Ascending(x => x);

        var timeSorter = SortExpressionComparer<QuestCompletionDto>
            .Descending(x => x.CompletionTime);

        ProfileNameSource = new();
        ProfileNameSource.Connect()
            .ObserveOn(_scheduler.Background)
            .Sort(stringSorter)
            .ObserveOn(_scheduler.Visual)
            .Bind(out _profileNames)
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

        this.WhenPropertyChanged(x => x.SelectedProfileName)
            .Do(x =>
            {
                ObjectiveNameSource.Edit(async x =>
                {
                    if (SelectedProfileName is null)
                        return;

                    x.Clear();
                    var result = await _questService.GetObjectiveNames(SelectedProfileName);
                    x.AddRange(result);
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
                    if (SelectedProfileName is null || SelectedObjectiveName is null)
                        return;

                    x.Clear();
                    var result = await _questService.GetBossQuestCompletions(SelectedProfileName, SelectedObjectiveName);
                    x.AddRange(result);
                });
            })
            .Subscribe()
            .DisposeWith(_cleanup);
    }

    public async Task InitializeProfiles()
    {
        var items = await _questService.GetObjectiveFamilies();
        ProfileNameSource.AddRange(items);

        SelectedProfileName = items.FirstOrDefault();
    }

    [RelayCommand]
    public async Task UpdateHistory()
    {
        if (string.IsNullOrEmpty(SelectedProfileName) || string.IsNullOrEmpty(SelectedObjectiveName))
            return;

        var items = await _questService.GetBossQuestCompletions(SelectedProfileName, SelectedObjectiveName);
        CompletionSource.Edit(x =>
        {
            x.Clear();
            x.AddRange(items);
        });
    }
}
