using NodaTime;
using RagnaRoute.Data;
using RagnaRoute.Scheduling;
using RagnaRoute.Objectives;

namespace RagnaRoute.ViewModels;
public static class Mappers
{
    public static ScheduledQuestViewModel ToViewModel(this ScheduledQuestModel model, ObjectiveStateDto? stateDto)
    {
        var followup = new CronSchedule(model.Repeat, model.TimeZone, Duration.FromMinutes(model.Duration));
        var objective = new ScheduledObjective(followup, stateDto?.LastCompletion, SystemClock.Instance);

        return new ScheduledQuestViewModel(model.ObjectiveName, objective)
        {
            Description = model.Description,
            Information = new(model.Information),
            IsHidden = stateDto?.IsHidden ?? false,
            TimeState = objective.State
        };
    }

    public static BossQuestViewModel ToViewModel(this BossQuestModel model, MonsterModel? monsterModel, ObjectiveStateDto? stateDto)
    {
        return new BossQuestViewModel(model.ObjectiveName, Duration.FromMinutes(model.MinimumRespawn), Duration.FromMinutes(model.MaximumRespawn), stateDto?.LastCompletion)
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
