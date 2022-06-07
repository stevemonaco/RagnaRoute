using NodaTime;
using RagnaRoute.Objectives;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace RagnaRoute.ViewModels;

public class KillQuestViewModel : ViewModelBase
{
    public ReactiveCommand<Unit, Unit> ResetObjectiveCommand { get; }

    public string Name { get; }
    public string? Description { get; init; }
    public ObservableCollection<string>? Information { get; init; }

    private int _timeUntilStarting;
    public int TimeUntilStarting
    {
        get => _timeUntilStarting;
        set => this.RaiseAndSetIfChanged(ref _timeUntilStarting, value);
    }

    private int _timeUntilEnding;
    public int TimeUntilEnding
    {
        get => _timeUntilEnding;
        set => this.RaiseAndSetIfChanged(ref _timeUntilEnding, value);
    }

    private TimeState _timeState;
    public TimeState TimeState
    {
        get => _timeState;
        set => this.RaiseAndSetIfChanged(ref _timeState, value);
    }

    public ScheduledObjective Objective { get; }

    public KillQuestViewModel(string name, ScheduledObjective objective)
    {
        Name = name;
        Objective = objective;
        ResetObjectiveCommand = ReactiveCommand.Create(ResetObjective);

        Objective.Reset(SystemClock.Instance.GetCurrentInstant());
    }

    public void UpdateObjective()
    {
        Objective.Update(SystemClock.Instance.GetCurrentInstant());

        TimeUntilStarting = (int)Math.Round(Objective.TimeUntilStarting.TotalSeconds, MidpointRounding.ToEven);
        TimeUntilEnding = (int)Math.Round(Objective.TimeUntilEnding.TotalSeconds, MidpointRounding.ToEven);
        TimeState = Objective.State;
    }

    public void ResetObjective()
    {

    }
}
