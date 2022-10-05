using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using DynamicData.Binding;
using RagnaRoute.Data;
using RagnaRoute.Services;
using RagnaRoute.ViewExtenders;

namespace RagnaRoute.ViewModels;

public partial class QuestHistoryViewModel : ViewModelBase, INavigationChild, IDisposable
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

    public SourceList<string> FamilyNameSource { get; }
    public SourceList<string> ObjectiveNameSource { get; }
    public SourceList<CompletionDto> CompletionSource { get; }

    private readonly ISchedulerProvider _scheduler;
    private readonly CompletionService _completionService;
    private readonly CompositeDisposable _cleanup = new();
    private bool _disposedValue;

    public QuestHistoryViewModel(ISchedulerProvider scheduler, CompletionService completionService)
    {
        _scheduler = scheduler;
        _completionService = completionService;

        var stringSorter = SortExpressionComparer<string>
            .Ascending(x => x);

        var timeSorter = SortExpressionComparer<CompletionDto>
            .Descending(x => x.CompletionTime);

        FamilyNameSource = new();
        FamilyNameSource.Connect()
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

        //this.WhenPropertyChanged(x => x.SelectedFamilyName)
        //    .ObserveOn(_scheduler.Visual)
        //    .Do(x =>
        //    {
        //        ObjectiveNameSource.Edit(async x =>
        //        {
        //            x.Clear();

        //            if (SelectedFamilyName is null)
        //                return;

        //            var result = await _completionService.GetCompletedObjectivesForFamily(SelectedFamilyName);
        //            x.AddRange(result.Select(x => x.ObjectiveName));
        //        });
        //    })
        //    .Subscribe()
        //    .DisposeWith(_cleanup);

        //this.WhenPropertyChanged(x => x.SelectedObjectiveName)
        //    .ObserveOn(_scheduler.Visual)
        //    .Do(x =>
        //    {
        //        CompletionSource.Edit(async x =>
        //        {
        //            x.Clear();

        //            if (SelectedFamilyName is null || SelectedObjectiveName is null)
        //                return;

        //            var result = await _completionService.GetCompletions(SelectedFamilyName, SelectedObjectiveName);
        //            x.AddRange(result);
        //            CompletionCount = result.Count;
        //        });
        //    })
        //    .Subscribe()
        //    .DisposeWith(_cleanup);
    }

    public async Task InitializeProfiles()
    {
        //await RefreshCommand.ExecuteAsync(null);

        var items = await _completionService.GetFamilyNames().ConfigureAwait(true);
        FamilyNameSource.AddRange(items);

        SelectedFamilyName = FamilyNameSource.Items.FirstOrDefault();
    }

    partial void OnSelectedFamilyNameChanged(string? value)
    {
        ObjectiveNameSource.Edit(async x =>
        {
            x.Clear();

            if (SelectedFamilyName is null)
                return;

            var result = await _completionService.GetCompletedObjectivesForFamily(SelectedFamilyName);
            x.AddRange(result.Select(x => x.ObjectiveName));
        });
    }

    partial void OnSelectedObjectiveNameChanged(string? value)
    {
        CompletionSource.Edit(async x =>
        {
            x.Clear();

            if (SelectedFamilyName is null || SelectedObjectiveName is null)
                return;

            var result = await _completionService.GetCompletions(SelectedFamilyName, SelectedObjectiveName);
            x.AddRange(result);
            CompletionCount = result.Count;
        });
    }

    [RelayCommand(AllowConcurrentExecutions = false)]
    public async Task Refresh()
    {
        var selectedFamilyName = SelectedFamilyName;
        var selectedObjectiveName = SelectedObjectiveName;

        ObjectiveNameSource.Clear();
        CompletionSource.Clear();
        FamilyNameSource.Clear();

        var items = await _completionService.GetFamilyNames().ConfigureAwait(true);
        FamilyNameSource.AddRange(items);

        await Task.Delay(1000);

        SelectedFamilyName = FamilyNameSource.Items.FirstOrDefault(x => x == selectedFamilyName) ?? FamilyNameSource.Items.FirstOrDefault();
        //OnPropertyChanged(nameof(SelectedFamilyName));

        await Task.Delay(1000);
        SelectedObjectiveName = ObjectiveNames.FirstOrDefault(x => x == selectedObjectiveName);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _cleanup.Dispose();
            }

            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
