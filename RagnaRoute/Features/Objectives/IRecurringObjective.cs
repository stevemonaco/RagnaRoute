using NodaTime;

namespace RagnaRoute.Objectives;
public interface IRecurringObjective : ITimedObjective
{
    /// <summary>
    /// Instant the objective was last reset
    /// </summary>
    Instant? LastReset { get; }

    /// <summary>
    /// Reset the object's LastReset time to the current time
    /// </summary>
    void Reset();

    /// <summary>
    /// Reset the objective's last reset time
    /// </summary>
    /// <param name="resetTime"></param>
    void Reset(Instant? resetTime);
}
