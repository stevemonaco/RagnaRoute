using CommunityToolkit.Mvvm.ComponentModel;
using NodaTime;
using RagnaRoute.Data;
using RagnaRoute.Objectives;

namespace RagnaRoute.ViewModels;
public partial class BossQuestViewModel : ViewModelBase, INavigationChild
{
    public string DisplayName => Name;
    public string Name { get; }
    public int? Id { get; init; }
    public long? HP { get; init; }
    public MonsterElement? Element { get; init; }
    public MonsterRace? Race { get; init; }
    public MonsterSize? Size { get; init; }
    public string? WarpLocation { get; init; }

    [ObservableProperty] private Duration _timeUntilStarting;
    [ObservableProperty] private Duration _timeUntilEnding;
    [ObservableProperty] private Duration? _timeSinceStarted;
    [ObservableProperty] private TimeState _timeState;
    [ObservableProperty] private bool _isHidden;

    public CooldownObjective Objective { get; }

    public BossQuestViewModel(string name, Duration minimumSpawnDuration, Duration maximumSpawnDuration, Instant? lastCompletion = null)
    {
        Name = name;

        Objective = new CooldownObjective(minimumSpawnDuration, maximumSpawnDuration, lastCompletion);
        _timeState = Objective.State;
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
            TimeUntilEnding = Duration.Zero;

            if (Objective.Prior?.HasEnd is true)
                TimeSinceStarted = current - Objective.Prior.Value.End;
            else if (Objective.Prior?.HasStart is true)
                TimeSinceStarted = current - Objective.Prior.Value.Start;
        }
        else if (Objective.State == TimeState.MaybeActive)
        {
            TimeUntilStarting = Duration.Zero;
            TimeUntilEnding = Objective.Upcoming!.Value.End - current;
            TimeSinceStarted = Duration.Zero;
        }

        TimeState = Objective.State;
    }
}
