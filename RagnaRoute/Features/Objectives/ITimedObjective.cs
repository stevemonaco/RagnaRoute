using NodaTime;

namespace RagnaRoute.Objectives;
public interface ITimedObjective
{
    /// <summary>
    /// When the objective starts
    /// </summary>
    Interval Start { get; }

    /// <summary>
    /// When the objective ends
    /// If null, then the objective ends immediately end after starting
    /// </summary>
    Interval? End { get; }

    /// <summary>
    /// Duration from last update until the objective will start
    /// </summary>
    Duration TimeUntilStarting { get; }

    /// <summary>
    /// Duration from last update until the objective will end
    /// </summary>
    Duration TimeUntilEnding { get; }

    /// <summary>
    /// Update the objective with the current time
    /// </summary>
    /// <param name="current"></param>
    void Update(Instant current);
}
