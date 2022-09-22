using NodaTime;

namespace RagnaRoute.Scheduling;

public interface IPrior
{
    /// <summary>
    /// Gets the Interval that starts before the given time
    /// </summary>
    /// <param name="instant"></param>
    /// <returns>The previous Interval or null if there is no next Interval</returns>
    Interval? Previous(Instant instant);
}
