using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using NaturalSort.Extension;
using NodaTime;
using RagnaRoute.Objectives;
using RagnaRoute.Services;
using RagnaRoute.ViewExtenders;

namespace RagnaRoute.ViewModels;

public partial class ScheduledQuestTrackingViewModel : TrackingGroupViewModel
{
    public ReadOnlyObservableCollection<ScheduledQuestViewModel> Quests { get => _quests; }
    private ReadOnlyObservableCollection<ScheduledQuestViewModel> _quests;
    public SourceList<ScheduledQuestViewModel> QuestSource { get; }

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

    public ScheduledQuestTrackingViewModel(IEnumerable<ScheduledQuestViewModel> quests, ISchedulerProvider scheduler, CompletionService completionService)
    {
        _scheduler = scheduler;
        _completionService = completionService;

        var filterTextChanged = this.WhenValueChanged(x => x.FilterText)
            .Select(CreateTextFilter);

        var sorter = SortExpressionComparer<ScheduledQuestViewModel>
            .Ascending(x => _timeStateSortingPriority(x.TimeState))
            .ThenByAscending(x => x.TimeUntilStarting)
            .ThenBy(x => x.Name, StringComparison.OrdinalIgnoreCase.WithNaturalSort());

        QuestSource = new();
        QuestSource.AddRange(quests);

        QuestSource.Connect()
            .ObserveOn(_scheduler.Background)
            .AutoRefresh(x => x.TimeState)
            .AutoRefresh(x => x.IsHidden)
            .AutoRefreshOnObservable(_ => this.WhenValueChanged(x => x.ShouldShowHidden))
            .Filter(x => !x.IsHidden || ShouldShowHidden)
            .Filter(filterTextChanged)
            .Sort(sorter)
            .ObserveOn(_scheduler.Visual)
            .Bind(out _quests)
            .Subscribe()
            .DisposeWith(_cleanup);
    }

    private Func<ScheduledQuestViewModel, bool> CreateTextFilter(string? text)
    {
        if (text is string { Length: >= 2 })
            return boss => boss.Name.Contains(text, StringComparison.OrdinalIgnoreCase);

        return boss => true;
    }

    public override void UpdateObjective()
    {
        foreach (var quest in Quests)
            quest.UpdateObjective();
    }

    [RelayCommand(AllowConcurrentExecutions = false)]
    public async Task CompleteObjectiveCommand(ScheduledQuestViewModel viewModel)
    {
        if (viewModel.Objective.Complete())
        {
            var instant = SystemClock.Instance.GetCurrentInstant();
            await _completionService.AddCompletion(Name, viewModel.Name, instant);
        }
    }
}
