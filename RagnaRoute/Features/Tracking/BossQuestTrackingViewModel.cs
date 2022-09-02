using System;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using NodaTime;
using RagnaRoute.Objectives;
using RagnaRoute.ViewExtenders;
using RagnaRoute.Services;

namespace RagnaRoute.ViewModels;
public partial class BossQuestTrackingViewModel : TrackingGroupViewModel, IDisposable
{
    public ReadOnlyObservableCollection<BossQuestViewModel> Bosses { get => _bosses; }
    private ReadOnlyObservableCollection<BossQuestViewModel> _bosses;
    public SourceList<BossQuestViewModel> BossSource { get; }

    [ObservableProperty] private bool _shouldShowHidden;
    [ObservableProperty] private string _filterText = string.Empty;

    private static Func<TimeState, int> _timeStatePriority = (TimeState state) => state switch
    {
        TimeState.During => 0,
        TimeState.After => 1,
        TimeState.Before => 2,
        TimeState.Completed => 3,
        TimeState.Indeterminate => 4,
        _ => throw new InvalidOperationException()
    };

    private readonly ISchedulerProvider _scheduler;
    private readonly CompletionService _completionService;

    private readonly IDisposable _cleanup;
    private bool _disposedValue;

    public BossQuestTrackingViewModel(IEnumerable<BossQuestViewModel> bosses, ISchedulerProvider scheduler, CompletionService completionService)
    {
        _scheduler = scheduler;
        _completionService = completionService;
        var filterTextChanged = this.WhenValueChanged(x => x.FilterText)
            .Select(CreateTextFilter);

        var sorter = SortExpressionComparer<BossQuestViewModel>
            .Ascending(x => _timeStatePriority(x.TimeState))
            .ThenByAscending(x => x.TimeUntilStarting)
            .ThenByAscending(x => x.Name);

        BossSource = new();
        BossSource.AddRange(bosses);

        BossSource.Connect()
            .ObserveOn(_scheduler.Background)
            .AutoRefresh(x => x.TimeState)
            .AutoRefresh(x => x.IsHidden)
            .AutoRefreshOnObservable(_ => this.WhenValueChanged(x => x.ShouldShowHidden))
            .Filter(x => !x.IsHidden || ShouldShowHidden)
            .Filter(filterTextChanged)
            .Sort(sorter)
            .ObserveOn(_scheduler.Visual)
            .Bind(out _bosses)
            .Subscribe();

        _cleanup = new CompositeDisposable(BossSource);
    }

    private Func<BossQuestViewModel, bool> CreateTextFilter(string? text)
    {
        if (text is string { Length: >= 2} )
            return boss => boss.Name.Contains(text, StringComparison.OrdinalIgnoreCase);

        return boss => true;
    }

    public override void UpdateObjective()
    {
        foreach (var boss in BossSource.Items.Where(x => x.TimeState != TimeState.Indeterminate))
        {
            boss.UpdateObjective();
        }
    }

    [RelayCommand(AllowConcurrentExecutions = true)]
    public async Task RecurObjective(BossQuestViewModel viewModel)
    {
        var instant = SystemClock.Instance.GetCurrentInstant();
        viewModel.Objective.Recur(instant);
        viewModel.UpdateObjective();

        await _completionService.AddCompletion(Name, viewModel.Name, instant);
    }

    [RelayCommand(AllowConcurrentExecutions = true)]
    public async Task ToggleHidden(BossQuestViewModel viewModel)
    {
        viewModel.IsHidden = !viewModel.IsHidden;
        await _completionService.UpsertObjectiveHiddenState(Name, viewModel.Name, viewModel.IsHidden);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _cleanup?.Dispose();
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
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
