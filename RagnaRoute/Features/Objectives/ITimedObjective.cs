using NodaTime;

namespace RagnaRoute.Objectives;

public enum TimeState { Before, During, After, Completed, Indeterminate }

public interface ITimedObjective
{
    /// <summary>
    /// When the Objective starts
    /// </summary>
    Instant Start { get; }

    /// <summary>
    /// Duration where the Objective is active
    /// </summary>
    Duration Duration { get; }

    /// <summary>
    /// When the Objective ends
    /// </summary>
    Instant End { get; }

    /// <summary>
    /// Duration from last update until the Objective will start
    /// </summary>
    Duration TimeUntilStarting { get; }

    /// <summary>
    /// Duration from last update until the Objective will end
    /// </summary>
    Duration TimeUntilEnding { get; }

    /// <summary>
    /// TimeState the Objective is currently in
    /// </summary>
    TimeState State { get; }

    /// <summary>
    /// Update the Objective with the current time
    /// </summary>
    /// <param name="current"></param>
    void Update(Instant current);
}
