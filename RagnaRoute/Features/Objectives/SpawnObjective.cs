//using System;
//using NodaTime;
//using RagnaRoute.Scheduling;

//namespace RagnaRoute.Objectives;
//public class SpawnObjective : IRecurringObjective
//{
//    public Instant Start { get; private set; }
//    public Duration Duration { get; private set; }
//    public Instant End => Start + Duration;

//    public Duration TimeUntilStarting { get; private set; }
//    public Duration TimeUntilEnding { get; private set; }

//    public Instant? LastCompletion { get; private set; }
//    public Instant? LastRecurrence { get; private set; }

//    public TimeState State { get; private set; } = TimeState.Indeterminate;

//    private readonly IFollowup _followup;
//    private readonly Duration _respawnMinimum;
//    private readonly Duration _respawnMaximum;
//    private Instant? _lastUpdate;

//    public SpawnObjective(Duration respawnTime) : this(respawnTime, respawnTime)
//    {
//    }

//    public SpawnObjective(Duration respawnMinimum, Duration respawnMaximum)
//    {
//        _followup = Followup.OnInterval(respawnMinimum, respawnMaximum);
//    }

//    public void Update() => Update(SystemClock.Instance.GetCurrentInstant());

//    /// <inheritdoc/>
//    public void Update(Instant current)
//    {
//        _lastUpdate = current;
//        UpdateRemaining(current);
//    }

//    /// <inheritdoc/>
//    public void Skip()
//    {
//        throw new NotImplementedException();
//    }

//    /// <inheritdoc/>
//    public void Complete() =>
//        Complete(SystemClock.Instance.GetCurrentInstant());

//    /// <inheritdoc/>
//    public void Complete(Instant? completionTime)
//    {
//        LastCompletion = completionTime;
//        State = TimeState.Completed;
//    }

//    public void Recur() =>
//        Recur(SystemClock.Instance.GetCurrentInstant());

//    public void Recur(Instant recurTime)
//    {
//        LastRecurrence = recurTime;

//        if (_lastUpdate is not null)
//            UpdateRemaining(_lastUpdate.Value);
//    }

//    //public void Reset()
//    //{
//    //    State = TimeState.Indeterminate;
//    //    LastCompletion = null;
//    //    UpdateRemaining(SystemClock.Instance.GetCurrentInstant());
//    //}

//    //public void Reset(Instant instant)
//    //{
//    //    State = TimeState.Indeterminate;
//    //    LastCompletion = instant;
//    //    UpdateRemaining(SystemClock.Instance.GetCurrentInstant());
//    //}

//    private void UpdateRemaining(Instant current)
//    {
//        if (State == TimeState.Completed || LastRecurrence is null)
//            return;

//        var next = _followup.Next(LastRecurrence.Value);
//        if (!next.HasValue)
//            return;

//        TimeUntilStarting = next.Value.Start - current;
//        TimeUntilEnding = next.Value.End - current;

//        if (HasExceededSpawnDuration())
//        {
//            State = TimeState.Indeterminate;
//            LastRecurrence = null;
//            return;
//        }

//        State = TimeHelpers.DetermineTimeState(TimeUntilStarting, TimeUntilEnding);
//    }

//    private bool HasExceededSpawnDuration()
//    {
//        int timeFactor = -2;
//        return TimeUntilEnding < _respawnMaximum * timeFactor;
//    }
//}
