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
        Objective.Update(SystemClock.Instance.GetCurrentInstant());

        TimeUntilStarting = Objective.TimeUntilStarting;
        TimeUntilEnding = Objective.TimeUntilEnding;
        TimeState = Objective.State;
    }
}
