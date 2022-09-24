using NodaTime;
using RagnaRoute.Scheduling;
using RagnaRoute.Objectives.Extensions;

namespace RagnaRoute.Objectives;

public class CooldownObjective : ITimedObjective
{
    public Interval? Prior { get; protected set; }
    public Interval? Ongoing { get; protected set; }
    public Interval? Upcoming { get; protected set; }
    public TimeState State { get; protected set; }
    public Instant? LastCompletion { get; protected set; }
    public ObjectiveResult? PriorResult { get; protected set; }

    public bool IsInCooldown { get; protected set; }

    private readonly IClock _clock;
    private readonly IFollowup _followup;
    private readonly Duration _cooldownMinimum;
    private readonly Duration _cooldownMaximum;
    private const double _inactiveFactor = 3;

    public CooldownObjective(Duration cooldownMinimum, Duration cooldownMaximum, Instant? lastCompletion = null, IClock? clock = null)
    {
        _cooldownMinimum = cooldownMinimum;
        _cooldownMaximum = cooldownMaximum;
        _followup = Followup.OnInterval(_cooldownMinimum, _cooldownMaximum);
        _clock = clock ?? SystemClock.Instance;

        LastCompletion = lastCompletion;
        InitializeState();
    }

    private void InitializeState()
    {
        var current = _clock.GetCurrentInstant();

        if (LastCompletion is null)
        {
            State = TimeState.Inactive;
            Ongoing = new Interval(current, null);
            return;
        }

        Prior = new Interval(LastCompletion.Value, LastCompletion.Value);
        var priorNext = _followup.Next(LastCompletion.Value);

        if (current.IsBefore(priorNext))
        {
            State = TimeState.AwaitingUpcoming;
            PriorResult = ObjectiveResult.Completed;
            Upcoming = new Interval(LastCompletion.Value + _cooldownMinimum, LastCompletion.Value + _cooldownMaximum);
        }
        else if (current.IsWithin(priorNext))
        {
            State = TimeState.Active;
            PriorResult = ObjectiveResult.Completed;
            Ongoing = new Interval(LastCompletion.Value + _cooldownMaximum, null);
        }
        else if (current.IsAfter(priorNext))
        {
            if (HasFarExceededCooldownDuration(current))
            {
                State = TimeState.Inactive;
                PriorResult = ObjectiveResult.Missed;
                Ongoing = new Interval(LastCompletion.Value + _cooldownMaximum, null);
            }
            else
            {
                State = TimeState.Active;
                PriorResult = ObjectiveResult.Completed;
                Ongoing = new Interval(LastCompletion.Value + _cooldownMaximum, null);
            }
        }
    }

    public virtual bool Update(Instant current)
    {
        var previousState = State;

        if (State == TimeState.AwaitingUpcoming && current.IsWithin(Upcoming))
        {
            Prior = Ongoing;
            Ongoing = Upcoming;
            State = TimeState.MaybeActive;
        }
        else if (State == TimeState.AwaitingUpcoming && current.IsAfter(Upcoming))
        {
            Prior = Ongoing;
            Ongoing = Upcoming;
            Upcoming = _followup.Next(current);

            State = TimeState.Active;
        }
        else if (State == TimeState.MaybeActive && current.IsAfter(Upcoming))
        {
            Prior = Ongoing;
            Ongoing = Upcoming;
            Upcoming = _followup.Next(current);

            State = TimeState.Active;
        }
        else if (State == TimeState.Active && HasFarExceededCooldownDuration(current))
        {
            Upcoming = _followup.Next(current);
            PriorResult = ObjectiveResult.Missed;

            if (Upcoming is null)
                State = TimeState.Ended;
            else
                State = TimeState.Inactive;
        }

        return State == previousState;
    }

    public virtual bool Complete()
    {
        if (!(State == TimeState.Active || State == TimeState.Inactive || State == TimeState.MaybeActive))
            return false;

        var current = _clock.GetCurrentInstant();

        Prior = new Interval(Ongoing?.Start ?? current, current);
        Ongoing = null;
        Upcoming = _followup.Next(current);
        PriorResult = ObjectiveResult.Completed;

        if (Upcoming is null)
            State = TimeState.Ended;
        else
            State = TimeState.AwaitingUpcoming;

        return true;
    }

    private bool HasFarExceededCooldownDuration(Instant current)
    {
        if (Prior?.HasEnd is true)
        {
            
            var delta = current - Prior.Value.End;

            return delta > _cooldownMaximum * _inactiveFactor;
        }

        return false;
    }
}
