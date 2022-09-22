using NodaTime;
using NodaTime.Extensions;
using RagnaRoute.Data;
using RagnaRoute.Scheduling;
using RagnaRoute.Objectives;

namespace RagnaRoute.ViewModels;
public static class Mappers
{
    public static ScheduledQuestViewModel ToViewModel(this ScheduledQuestModel model)
    {
        //var zone = DateTimeZoneProviders.Tzdb.GetSystemDefault();
        //var clock = SystemClock.Instance.InTzdbSystemDefaultZone();
        //var localDate = clock.GetCurrentDate();
        //var localTime = new LocalTime(4, 0);

        //var zonedTime = new LocalDateTime(localDate.Year, localDate.Month, localDate.Day, localTime.Hour, localTime.Minute);

        //var instant = zonedTime.InZoneLeniently(zone).ToInstant();
        //var followup = Followup.OnDaily(localTime, Duration.FromSeconds(86399));

        var followup = new CronSchedule(model.Repeat, model.TimeZone, Duration.FromMinutes(model.Duration));
        var objective = new ScheduledObjective(followup);

        return new ScheduledQuestViewModel(model.Name, objective)
        {
            Description = model.Description,
            Information = new(model.Information)
        };
    }

    public static BossQuestViewModel ToViewModel(this BossQuestModel model, MonsterModel? monsterModel, ObjectiveStateDto? stateDto)
    {
        return new BossQuestViewModel(model.ObjectiveName, Duration.FromSeconds(model.MinimumRespawn), Duration.FromSeconds(model.MaximumRespawn), stateDto?.LastCompletion)
        {
            Id = model.MobId,
            HP = monsterModel?.HP,
            Element = monsterModel?.Element,
            Race = monsterModel?.Race,
            Size = monsterModel?.Size,
            WarpLocation = model.WarpLocation,
            IsHidden = stateDto?.IsHidden ?? false
        };
    }
}
