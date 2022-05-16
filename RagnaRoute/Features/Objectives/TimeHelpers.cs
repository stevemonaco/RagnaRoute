using System;
using NodaTime;

namespace RagnaRoute.Objectives;

internal static class TimeHelpers
{
    public static TimeState DetermineTimeState(Duration timeUntilStarting, Duration timeUntilEnding)
    {
        if (timeUntilStarting < Duration.Zero && timeUntilEnding < Duration.Zero)
            return TimeState.Completed;
        else if (timeUntilStarting <= Duration.Zero && timeUntilEnding > Duration.Zero)
            return TimeState.During;
        else if (timeUntilStarting > Duration.Zero && timeUntilEnding >= Duration.Zero)
            return TimeState.Before;
        else
            throw new InvalidOperationException($"{nameof(DetermineTimeState)} with a positive {nameof(timeUntilStarting)} '{timeUntilStarting}' and negative {nameof(timeUntilEnding)} '{timeUntilEnding}'");
    }
}
