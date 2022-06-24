using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RagnaRoute.Features.Schedule;

public interface IFollowup
{
    /// <summary>
    /// Gets the next Interval that starts after the given time
    /// </summary>
    /// <param name="instant"></param>
    /// <returns>The next Interval or null if there is no next Interval</returns>
    Interval? Next(Instant instant);
}
