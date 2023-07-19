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
//using ReactiveUI;

namespace RagnaRoute.ViewModels;
public sealed partial class BossQuestTrackingViewModel : TrackingGroupViewModel
{
    public ReadOnlyObservableCollection<BossQuestViewModel> Bosses { get => _bosses; }
    private ReadOnlyObservableCollection<BossQuestViewModel> _bosses;
    public SourceList<BossQuestViewModel> BossSource { get; }

    [ObservableProperty] private bool _shouldShowHidden;
    [ObservableProperty] private string _filterText = string.Empty;

    private static readonly Func<TimeState, int> _timeStateSortingPriority = (TimeState state) => state switch
    {
        TimeState.Active => 0,
        TimeState.MaybeActive => 1,
        TimeState.AwaitingUpcoming => 2,
        TimeState.Inactive => 3,
        TimeState.Ended => 4,
        _ => throw new InvalidOperationException()
    };

    private readonly ISchedulerProvider _scheduler;
    private readonly CompletionService _completionService;

    public BossQuestTrackingViewModel(IEnumerable<BossQuestViewModel> bosses, ISchedulerProvider scheduler, CompletionService completionService)
    {
        _scheduler = scheduler;
        _completionService = completionService;
        var filterTextChanged = this.WhenValueChanged(x => x.FilterText)
            .Select(CreateTextFilter);

        var sorter = SortExpressionComparer<BossQuestViewModel>
            .Ascending(x => _timeStateSortingPriority(x.TimeState))
            .ThenByAscending(x => x.TimeUntilStarting)
            //.ThenByAscending(x => x.TimeSinceStarted ?? Duration.Zero)
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
            .Subscribe()
            .DisposeWith(_cleanup);
    }

    private Func<BossQuestViewModel, bool> CreateTextFilter(string? text)
    {
        if (text is string { Length: >= 2} )
            return boss => boss.Name.Contains(text, StringComparison.OrdinalIgnoreCase);

        return boss => true;
    }

    public override void UpdateObjective()
    {
        foreach (var boss in BossSource.Items.Where(x => x.TimeState != TimeState.Ended))
        {
            boss.UpdateObjective();
        }
    }

    [RelayCommand(AllowConcurrentExecutions = true)]
    public async Task CompleteObjective(BossQuestViewModel viewModel)
    {
        if (viewModel.Objective.Complete() && viewModel.Objective.LastCompletion is Instant instant)
        {
            viewModel.UpdateObjective();
            await _completionService.AddCompletion(Name, viewModel.Name, instant);
        }
    }

    [RelayCommand(AllowConcurrentExecutions = true)]
    public async Task ToggleHidden(BossQuestViewModel viewModel)
    {
        viewModel.IsHidden = !viewModel.IsHidden;
        await _completionService.UpsertObjectiveHiddenState(Name, viewModel.Name, viewModel.IsHidden);
    }
}
