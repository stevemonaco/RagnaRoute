using NodaTime;

namespace RagnaRoute.Objectives.Extensions;

internal static class InstantExtensions
{
    public static bool IsBefore(this Instant instant, Interval? interval)
    {
        if (interval is null || !interval.Value.HasStart)
            return false;

        return instant < interval.Value.Start;
    }

    public static bool IsWithin(this Instant instant, Interval? interval)
    {
        if (interval is null || !interval.Value.HasStart || !interval.Value.HasEnd)
            return false;

        return instant >= interval.Value.Start && instant <= interval.Value.End;
    }

    public static bool IsAfter(this Instant instant, Interval? interval)
    {
        if (interval is null || !interval.Value.HasEnd)
            return false;

        return instant > interval.Value.End;
    }

    public static bool IsBefore(this Instant a, Instant b) => a < b;
    public static bool IsWithin(this Instant instant, Instant low, Instant high) => instant >= low && instant <= high;
    public static bool IsAfter(this Instant a, Instant b) => a > b;
}
