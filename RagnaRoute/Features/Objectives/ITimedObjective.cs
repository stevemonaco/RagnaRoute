using NodaTime;

namespace RagnaRoute.Objectives;

//public enum TimeState { Before, During, After, Completed, Indeterminate }
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

//public interface ITimedObjective
//{
//    /// <summary>
//    /// When the Objective starts
//    /// </summary>
//    Instant Start { get; }

//    /// <summary>
//    /// Duration where the Objective is active
//    /// </summary>
//    Duration Duration { get; }

//    /// <summary>
//    /// When the Objective ends
//    /// </summary>
//    Instant End { get; }

//    /// <summary>
//    /// Duration from last update until the Objective will start
//    /// </summary>
//    Duration TimeUntilStarting { get; }

//    /// <summary>
//    /// Duration from last update until the Objective will end
//    /// </summary>
//    Duration TimeUntilEnding { get; }

//    /// <summary>
//    /// Instant the Objective was last completed
//    /// </summary>
//    Instant? LastCompletion { get; }

//    /// <summary>
//    /// Current TimeState of the Objective
//    /// </summary>
//    TimeState State { get; }

//    /// <summary>
//    /// Update the Objective with the current time
//    /// </summary>
//    /// <param name="current"></param>
//    void Update(Instant current);
//}
