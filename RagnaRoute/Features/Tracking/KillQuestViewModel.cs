using System;
using System.Collections.ObjectModel;
using NodaTime;
using RagnaRoute.Objectives;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;

namespace RagnaRoute.ViewModels;

public partial class KillQuestViewModel : ViewModelBase
{
    public string Name { get; }
    public string? Description { get; init; }
    public ObservableCollection<string>? Information { get; init; }

    [ObservableProperty] private int _timeUntilStarting;
    [ObservableProperty] private int _timeUntilEnding;
    [ObservableProperty] private TimeState _timeState;

    public ScheduledObjective Objective { get; }

    public KillQuestViewModel(string name, ScheduledObjective objective)
    {
        Name = name;
        Objective = objective;

        //Objective.Complete(SystemClock.Instance.GetCurrentInstant());
    }

    public void UpdateObjective()
    {
        Objective.Update(SystemClock.Instance.GetCurrentInstant());

        TimeUntilStarting = (int)Math.Round(Objective.TimeUntilStarting.TotalSeconds, MidpointRounding.ToEven);
        TimeUntilEnding = (int)Math.Round(Objective.TimeUntilEnding.TotalSeconds, MidpointRounding.ToEven);
        TimeState = Objective.State;
    }

    [RelayCommand]
    public void CompleteObjective()
    {
        Objective.Complete();
        UpdateObjective();
    }
}
