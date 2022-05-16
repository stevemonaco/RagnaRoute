using NodaTime;
using RagnaRoute.Model;
using RagnaRoute.Objectives;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace RagnaRoute.ViewModels;
public class BossViewModel : ViewModelBase
{
    public ReactiveCommand<Unit, Unit> ResetObjectiveCommand { get; }

    public string Name { get; }
    public long HP { get; set; }
    public MonsterElement Element { get; set; }
    public MonsterRace Race { get; set; }
    public MonsterSize Size { get; set; }

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

    public SpawnObjective Objective { get; }

    public BossViewModel(string name, long hp, MonsterElement element, MonsterRace race, MonsterSize size)
    {
        Name = name;
        HP = hp;
        Element = element;
        Race = race;
        Size = size;

        ResetObjectiveCommand = ReactiveCommand.Create(ResetObjective);

        Objective = new SpawnObjective(Duration.FromSeconds(15), Duration.FromSeconds(25));
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
        Objective.Reset(SystemClock.Instance.GetCurrentInstant());
        UpdateObjective();
    }
}
