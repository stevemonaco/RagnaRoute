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

namespace RagnaRoute.ViewModels;
public class BossQuestTrackingViewModel : TrackingGroupViewModel
{
    public ReadOnlyObservableCollection<BossQuestViewModel> Bosses { get => _bosses; }
    private ReadOnlyObservableCollection<BossQuestViewModel> _bosses;

    public SourceList<BossQuestViewModel> BossSource { get; }

    public BossQuestTrackingViewModel(IEnumerable<BossQuestViewModel> bosses)
    {
        Name = "Field Bosses";

        BossSource = new();
        BossSource.AddRange(bosses);

        BossSource.Connect()
            //.ObserveOn(RxApp.MainThreadScheduler)
            .AutoRefresh()
            //.AutoRefresh(x => x.IsHidden, propertyChangeThrottle: TimeSpan.FromMilliseconds(500))
            //.AutoRefresh(x => x.TimeUntilStarting, propertyChangeThrottle: TimeSpan.FromMilliseconds(1000))
            .Filter(x => !x.IsHidden)
            .Sort(SortExpressionComparer<BossQuestViewModel>.Descending(x => x.TimeUntilStarting).ThenByAscending(x => x.Name))
            .Bind(out _bosses)
            .DisposeMany()
            .Subscribe();
    }

    public override void UpdateObjective()
    {
        foreach (var boss in BossSource.Items)
        {
            boss.UpdateObjective();
        }
    }
}
