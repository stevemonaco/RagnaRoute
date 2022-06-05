using Avalonia.Threading;
using RagnaRoute.Data;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;

namespace RagnaRoute.ViewModels;
public class ShellViewModel : ViewModelBase
{
    private ObservableCollection<TrackingGroupViewModel> _trackers;
    public ObservableCollection<TrackingGroupViewModel> Trackers
    {
        get => _trackers;
        set => this.RaiseAndSetIfChanged(ref _trackers, value);
    }

    private readonly DispatcherTimer _objectiveTimer;

    public ShellViewModel(MonsterStore monsterStore)
    {
        _trackers = new()
        {
            new BossTrackingViewModel(monsterStore),
            new KillQuestTrackingViewModel(),
            new KillQuestTrackingViewModel()
        };

        _objectiveTimer = new DispatcherTimer(TimeSpan.FromSeconds(1), DispatcherPriority.Normal, ObjectiveTimer_Tick);
        _objectiveTimer.Start();
    }

    private void ObjectiveTimer_Tick(object? sender, EventArgs e)
    {
        foreach (var tracker in Trackers)
            tracker.UpdateObjective();
    }
}
