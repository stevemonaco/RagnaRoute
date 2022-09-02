using NodaTime;

namespace RagnaRoute.Objectives;
public interface IRecurringObjective : ITimedObjective
{
    /// <summary>
    /// Instant the objective was last completed
    /// </summary>
    Instant? LastCompletion { get; }

    /// <summary>
    /// Moves the Objective to the next Followup without completing it
    /// </summary>
    void Next();

    /// <summary>
    /// Completes the objective at the current time
    /// </summary>
    void Complete();

    /// <summary>
    /// Completes the objective at the given time
    /// </summary>
    /// <param name="completionTime"></param>
    void Complete(Instant? completionTime);

    /// <summary>
    /// Resets the objective as if the last recurrence was at the given time
    /// </summary>
    //void Reset(Instant? resetTime);
}
