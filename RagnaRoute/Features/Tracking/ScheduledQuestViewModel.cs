using System;
using System.Collections.ObjectModel;
using NodaTime;
using RagnaRoute.Objectives;
using CommunityToolkit.Mvvm.ComponentModel;

namespace RagnaRoute.ViewModels;

public partial class ScheduledQuestViewModel : ViewModelBase
{
    public string Name { get; }
    public string? Description { get; init; }
    public ObservableCollection<string>? Information { get; init; }

    [ObservableProperty] private Duration _timeUntilStarting;
    [ObservableProperty] private Duration _timeUntilEnding;
    [ObservableProperty] private Duration _timeSinceStarted;
    [ObservableProperty] private TimeState _timeState;
    [ObservableProperty] private bool _isHidden;

    public ScheduledObjective Objective { get; }

    public ScheduledQuestViewModel(string name, ScheduledObjective objective)
    {
        Name = name;
        Objective = objective;
    }

    public void UpdateObjective()
    {
        var clock = SystemClock.Instance;
        var current = clock.GetCurrentInstant();

        Objective.Update(current);

        if (Objective.State == TimeState.AwaitingUpcoming)
        {
            TimeUntilStarting = Objective.Upcoming!.Value.Start - current;
            TimeUntilEnding = Objective.Upcoming!.Value.End - current;
            TimeSinceStarted = Duration.Zero;
        }
        else if (Objective.State == TimeState.Active)
        {
            TimeUntilStarting = Duration.Zero;
            TimeUntilEnding = Objective.Ongoing!.Value.End - current;

            if (Objective.Prior?.HasEnd is true)
                TimeSinceStarted = current - Objective.Prior.Value.End;
            else if (Objective.Prior?.HasStart is true)
                TimeSinceStarted = current - Objective.Prior.Value.Start;
        }

        TimeState = Objective.State;
    }
}
