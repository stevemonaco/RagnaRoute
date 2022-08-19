using System;
using System.Collections.ObjectModel;
using System.Linq;
using ReactiveUI;
using System.Collections.Generic;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using CommunityToolkit.Mvvm.ComponentModel;
using Avalonia.Threading;
using RagnaRoute.Objectives;
using System.Reactive.Concurrency;
using RagnaRoute.ViewExtenders;
using NodaTime;

namespace RagnaRoute.ViewModels;
public partial class BossQuestTrackingViewModel : TrackingGroupViewModel
{
    public ReadOnlyObservableCollection<BossQuestViewModel> Bosses { get => _bosses; }
    private ReadOnlyObservableCollection<BossQuestViewModel> _bosses;
    public SourceList<BossQuestViewModel> BossSource { get; }

    [ObservableProperty] private bool _shouldShowHidden;
    [ObservableProperty] private string _filterText = string.Empty;

    private static Func<TimeState, int> _timeStateOrder = (TimeState state) => state switch
    {
        TimeState.During => 0,
        TimeState.After => 1,
        TimeState.Before => 2,
        TimeState.Completed => 3,
        TimeState.Indeterminate => 4,
        _ => throw new InvalidOperationException()
    };

    private readonly ISchedulerProvider _scheduler;

    public BossQuestTrackingViewModel(IEnumerable<BossQuestViewModel> bosses, ISchedulerProvider scheduler)
    {
        Name = "Field Bosses";
        _scheduler = scheduler;

        var filterTextChanged = this.WhenValueChanged(x => x.FilterText)
            .Select(CreateTextFilter);

        var sorter = SortExpressionComparer<BossQuestViewModel>
            .Ascending(x => _timeStateOrder(x.TimeState))
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
            .ObserveOn(_scheduler.Main)
            .Bind(out _bosses)
            .Subscribe();
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
}
