using NodaTime;

namespace RagnaRoute.Scheduling;

public interface IFollowup
{
    /// <summary>
    /// Gets the next Interval that starts after the given time
    /// </summary>
    /// <param name="instant"></param>
    /// <returns>The next Interval or null if there is no next Interval</returns>
    Interval? Next(Instant instant);
}
