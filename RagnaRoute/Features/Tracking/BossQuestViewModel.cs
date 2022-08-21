using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NodaTime;
using RagnaRoute.Data;
using RagnaRoute.Objectives;

namespace RagnaRoute.ViewModels;
public partial class BossQuestViewModel : ViewModelBase
{
    public string Name { get; }
    public int? Id { get; init; }
    public long? HP { get; init; }
    public MonsterElement? Element { get; init; }
    public MonsterRace? Race { get; init; }
    public MonsterSize? Size { get; init; }
    public string? WarpLocation { get; init; }

    [ObservableProperty] private Duration _timeUntilStarting;
    [ObservableProperty] private Duration _timeUntilEnding;
    [ObservableProperty] private TimeState _timeState;
    [ObservableProperty] private bool _isHidden;

    public SpawnObjective Objective { get; }

    public BossQuestViewModel(string name, Duration minimumSpawnDuration, Duration maximumSpawnDuration)
    {
        Name = name;
        Objective = new SpawnObjective(minimumSpawnDuration, maximumSpawnDuration);
        _timeState = Objective.State;
    }

    public void UpdateObjective()
    {
        Objective.Update(SystemClock.Instance.GetCurrentInstant());

        TimeUntilStarting = Objective.TimeUntilStarting;
        TimeUntilEnding = Objective.TimeUntilEnding;
        TimeState = Objective.State;
    }

    //[RelayCommand]
    //public void RecurObjective()
    //{
    //    Objective.Recur(SystemClock.Instance.GetCurrentInstant());
    //    UpdateObjective();
    //}

    //[RelayCommand]
    //public void ResetObjective()
    //{
    //    Objective.Complete(SystemClock.Instance.GetCurrentInstant());
    //    UpdateObjective();
    //}
}
