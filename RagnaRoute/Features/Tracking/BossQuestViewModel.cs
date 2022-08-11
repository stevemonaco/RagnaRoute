using NodaTime;
using RagnaRoute.Data;
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
public class BossQuestViewModel : ViewModelBase
{
    public ReactiveCommand<Unit, Unit> ResetObjectiveCommand { get; }

    public string Name { get; }
    public int? Id { get; init; }
    public long? HP { get; init; }
    public MonsterElement? Element { get; init; }
    public MonsterRace? Race { get; init; }
    public MonsterSize? Size { get; init; }
    public ObservableCollection<string> Information { get; init; }

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

    private bool _isHidden;
    public bool IsHidden
    {
        get => _isHidden;
        set => this.RaiseAndSetIfChanged(ref _isHidden, value);
    }

    public SpawnObjective Objective { get; }

    public BossQuestViewModel(string name, Duration minimumSpawnDuration, Duration maximumSpawnDuration)
    {
        Name = name;

        ResetObjectiveCommand = ReactiveCommand.Create(ResetObjective, outputScheduler: RxApp.TaskpoolScheduler);

        Objective = new SpawnObjective(minimumSpawnDuration, maximumSpawnDuration);
        //Objective.Complete(SystemClock.Instance.GetCurrentInstant());
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
        Objective.Complete(SystemClock.Instance.GetCurrentInstant());
        UpdateObjective();
    }
}
