using NodaTime;

namespace RagnaRoute.Objectives;
public class OneTimeObjective : ITimedObjective
{
    public Instant Start { get; }
    public Duration Duration { get; }

    public Instant End => Start + Duration;

    public Duration TimeUntilStarting { get; private set; }
    public Duration TimeUntilEnding { get; private set; }
    public TimeState State { get; private set; } = TimeState.Indeterminate;

    public OneTimeObjective(Instant start) : this(start, Duration.Zero)
    {
    }

    public OneTimeObjective(Instant start, Duration duration)
    {
        Start = start;
        Duration = duration;

        Update(SystemClock.Instance.GetCurrentInstant());
    }

    public void Update(Instant current)
    {
        TimeUntilStarting = Start - current;
        TimeUntilEnding = End - current;

        if (State != TimeState.Completed)
            State = TimeHelpers.DetermineTimeState(TimeUntilStarting, TimeUntilEnding);
    }

    /// <summary>
    /// Completes the Objective
    /// </summary>
    public void Complete()
    {
        State = TimeState.Completed;
        TimeUntilStarting = Duration.MaxValue;
        TimeUntilEnding = Duration.MaxValue;
    }
}
