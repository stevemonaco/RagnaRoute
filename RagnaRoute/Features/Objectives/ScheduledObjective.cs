using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RagnaRoute.Objectives;
public class ScheduledObjective : IRecurringObjective
{
    public Instant Start { get; private set; }
    public Duration Duration { get; private set; }

    public Instant End => Start + Duration;

    public Duration TimeUntilStarting { get; private set; }
    public Duration TimeUntilEnding { get; private set; }

    public Instant? LastReset { get; private set; }

    public TimeState State => throw new NotImplementedException();

    public ScheduledObjective()
    {

    }

    public void Reset()
    {
        throw new NotImplementedException();
    }

    public void Reset(Instant? resetTime)
    {
        throw new NotImplementedException();
    }

    public void Update(Instant current)
    {
        throw new NotImplementedException();
    }
}
