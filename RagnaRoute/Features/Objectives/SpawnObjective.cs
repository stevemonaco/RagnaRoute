using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RagnaRoute.Objectives;
public class SpawnObjective : IRecurringObjective
{
    public Interval Start { get; private set; }
    public Interval? End { get; private set; }

    public Duration TimeUntilStarting { get; private set; }
    public Duration TimeUntilEnding { get; private set; }

    public Instant? LastReset { get; private set; }

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
        if (LastReset is null)
            return;

        _lastUpdate = current;

        UpdateRemaining(current);
    }

    public void Reset()
    {
        LastReset = SystemClock.Instance.GetCurrentInstant();
    }

    /// <inheritdoc/>
    public void Reset(Instant? resetTime)
    {
        LastReset = resetTime;

        if (_lastUpdate is not null)
            UpdateRemaining(_lastUpdate.Value);
    }

    private void UpdateRemaining(Instant instant)
    {
        if (LastReset is null)
            return;

        TimeUntilStarting = _respawnMinimum - (instant - LastReset.Value);
        TimeUntilEnding = _respawnMaximum - (instant - LastReset.Value);
    }
}
