using NodaTime;
using RagnaRoute.Objectives.Extensions;

namespace RagnaRoute.Objectives;
public class OneTimeObjective : ITimedObjective
{
    public Interval? Prior { get; protected set; }
    public Interval? Ongoing { get; protected set; }
    public Interval? Upcoming { get; protected set; }
    public TimeState State { get; protected set; }
    public Instant? LastCompletion { get; protected set; }
    public ObjectiveResult? PriorResult { get; protected set; }

    private IClock _clock;

    public OneTimeObjective(Instant start, IClock? clock = null) : this(start, null, clock)
    {
    }

    public OneTimeObjective(Instant start, Duration duration, IClock? clock = null) : this(start, start + duration, clock)
    {
    }

    public OneTimeObjective(Interval start, IClock? clock = null) : this(start.Start, start.End, clock)
    {
    }

    public OneTimeObjective(Instant start, Instant? end, IClock? clock = null)
    {
        _clock = clock ?? SystemClock.Instance;
        var current = _clock.GetCurrentInstant();
        var interval = new Interval(start, end);

        if (interval.Contains(current))
        {
            State = TimeState.Active;
            Ongoing = interval;
        }
        else if (current.IsBefore(interval))
        {
            State = TimeState.AwaitingUpcoming;
            Upcoming = interval;
        }
        else if (current.IsAfter(interval))
        {
            State = TimeState.Ended;
            Prior = interval;
        }
    }

    public bool Update(Instant current)
    {
        var previousState = State;

        if (State == TimeState.AwaitingUpcoming && current.IsWithin(Upcoming))
        {
            Ongoing = Upcoming;
            Upcoming = null;

            State = TimeState.Active;
        }
        else if (State == TimeState.Active && current.IsAfter(Ongoing))
        {
            Prior = Ongoing;
            Ongoing = null;

            PriorResult = ObjectiveResult.Missed;
            State = TimeState.Ended;
        }

        return State == previousState;
    }

    /// <summary>
    /// Completes the Objective
    /// </summary>
    public bool Complete()
    {
        if (State != TimeState.Active)
            return false;

        State = TimeState.Ended;
        LastCompletion = _clock.GetCurrentInstant();
        PriorResult = ObjectiveResult.Completed;

        Prior = Ongoing;
        Ongoing = null;
        Upcoming = null;
        return true;
    }
}
