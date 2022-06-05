using NodaTime;
using RagnaRoute.Objectives;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace RagnaRoute.ViewModels;

public class KillQuestViewModel : ViewModelBase
{
    public ReactiveCommand<Unit, Unit> ResetObjectiveCommand { get; }

    public string Name { get; }

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

    public KillQuestViewModel(string name)
    {
        Name = name;

        Objective = new ScheduledObjective();
        Objective.Reset(SystemClock.Instance.GetCurrentInstant());
    }

    public void UpdateObjective()
    {
        Objective.Update(SystemClock.Instance.GetCurrentInstant());

        TimeUntilStarting = (int)Math.Round(Objective.TimeUntilStarting.TotalSeconds, MidpointRounding.ToEven);
        TimeUntilEnding = (int)Math.Round(Objective.TimeUntilEnding.TotalSeconds, MidpointRounding.ToEven);
        TimeState = Objective.State;
    }
}
