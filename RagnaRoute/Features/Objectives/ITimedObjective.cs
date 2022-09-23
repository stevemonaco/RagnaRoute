using NodaTime;

namespace RagnaRoute.Objectives;

public enum TimeState { AwaitingUpcoming, MaybeActive, Active, Ended, Inactive }
public enum ObjectiveResult { Missed, Completed }

public interface ITimedObjective
{
    Interval? Prior { get; }
    Interval? Ongoing { get; }
    Interval? Upcoming { get; }
    TimeState State { get; }
    ObjectiveResult? PriorResult { get; }
    Instant? LastCompletion { get; }

    bool Update(Instant current);
    bool Complete();
}