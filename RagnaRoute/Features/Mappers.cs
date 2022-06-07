using RagnaRoute.Features.Data;
using RagnaRoute.Objectives;

namespace RagnaRoute.ViewModels;
public static class Mappers
{
    public static KillQuestViewModel ToViewModel(this KillQuestModel model)
    {
        return new KillQuestViewModel(model.Name, new ScheduledObjective())
        {
            Description = model.Description,
            Information = new(model.Information)
        };
    }
}
