using System;
using NodaTime;

namespace RagnaRoute.Objectives;
public class SpawnObjective : IRecurringObjective
{
    public Instant Start { get; private set; }
    public Duration Duration { get; private set; }
    public Instant End => Start + Duration;

    public Duration TimeUntilStarting { get; private set; }
    public Duration TimeUntilEnding { get; private set; }

    public Instant? LastReset { get; private set; }

    public TimeState State { get; private set; } = TimeState.Indeterminate;

    private readonly Duration _respawnMinimum;
    private readonly Duration _respawnMaximum;
    private Instant? _lastUpdate;

    public SpawnObjective(Duration respawnTime) : this(respawnTime, respawnTime)
    {
    }

    public SpawnObjective(Duration respawnMinimum, Duration respawnMaximum)
    {
        _respawnMinimum = respawnMinimum;
        _respawnMaximum = respawnMaximum;
    }

    /// <inheritdoc/>
    public void Update(Instant current)
    {
        _lastUpdate = current;

        if (LastReset is not null)
            UpdateRemaining(current);
    }

    /// <inheritdoc/>
    public void Reset() =>
        Reset(SystemClock.Instance.GetCurrentInstant());

    /// <inheritdoc/>
    public void Reset(Instant? resetTime)
    {
        LastReset = resetTime;

        if (_lastUpdate is not null)
            UpdateRemaining(_lastUpdate.Value);
    }

    private void UpdateRemaining(Instant current)
    {
        if (LastReset is null)
            return;

        TimeUntilStarting = _respawnMinimum - (current - LastReset.Value);
        TimeUntilEnding = _respawnMaximum - (current - LastReset.Value);

        State = TimeHelpers.DetermineTimeState(TimeUntilStarting, TimeUntilEnding);
    }
}
