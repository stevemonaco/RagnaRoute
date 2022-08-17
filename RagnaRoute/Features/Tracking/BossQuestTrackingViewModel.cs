using System;
using System.Collections.ObjectModel;
using System.Linq;
using ReactiveUI;
using RagnaRoute.Data;
using System.Collections.Generic;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using DynamicData.Aggregation;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Threading;
using System.Reactive.Concurrency;
using Avalonia.Threading;
using RagnaRoute.Objectives;

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

    public BossQuestTrackingViewModel(IEnumerable<BossQuestViewModel> bosses)
    {
        Name = "Field Bosses";

        var filterTextChanged = this.WhenValueChanged(x => x.FilterText)
            .Select(CreateTextFilter);

        var sorter = SortExpressionComparer<BossQuestViewModel>
            .Ascending(x => _timeStateOrder(x.TimeState))
            .ThenByDescending(x => x.TimeUntilStarting)
            .ThenByAscending(x => x.Name);

        BossSource = new();
        BossSource.AddRange(bosses);
        
        BossSource.Connect()
            .AutoRefresh()
            .Throttle(TimeSpan.FromMilliseconds(200))
            .ObserveOn(AvaloniaScheduler.Instance)
            .AutoRefreshOnObservable(_ => this.WhenValueChanged(x => x.ShouldShowHidden))
            .Filter(x => !x.IsHidden || ShouldShowHidden)
            .Filter(filterTextChanged)
            .Sort(sorter)
            .Bind(out _bosses)
            .DisposeMany()
            .Subscribe();
    }

    private Func<BossQuestViewModel, bool> CreateTextFilter(string? text)
    {
        if (text is null || text.Length < 2)
            return boss => true;

        return boss => boss.Name.Contains(text, StringComparison.OrdinalIgnoreCase);
    }

    public override void UpdateObjective()
    {
        foreach (var boss in BossSource.Items)
        {
            boss.UpdateObjective();
        }
    }
}
