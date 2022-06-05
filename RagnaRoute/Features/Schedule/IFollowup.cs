using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RagnaRoute.Features.Schedule;

public interface IFollowup
{
    Interval Next(Instant current);
}
