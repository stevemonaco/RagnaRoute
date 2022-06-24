using NodaTime;
using NodaTime.Extensions;
using RagnaRoute.Data;
using RagnaRoute.Features.Schedule;
using RagnaRoute.Objectives;

namespace RagnaRoute.ViewModels;
public static class Mappers
{
    public static KillQuestViewModel ToViewModel(this KillQuestModel model)
    {
        var zone = DateTimeZoneProviders.Tzdb.GetSystemDefault();
        var clock = SystemClock.Instance.InTzdbSystemDefaultZone();
        var localDate = clock.GetCurrentDate();
        var localTime = new LocalTime(4, 0);

        var zonedTime = new LocalDateTime(localDate.Year, localDate.Month, localDate.Day, localTime.Hour, localTime.Minute);

        var instant = zonedTime.InZoneLeniently(zone).ToInstant();
        var followup = Followup.OnDaily(localTime, Duration.FromSeconds(86399));
        var objective = new ScheduledObjective(instant, followup);

        return new KillQuestViewModel(model.Name, objective)
        {
            Description = model.Description,
            Information = new(model.Information)
        };
    }
}
