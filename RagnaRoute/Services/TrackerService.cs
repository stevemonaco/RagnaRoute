using RagnaRoute.Data;
using RagnaRoute.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RagnaRoute.Services;
public class TrackerService
{
    private string _trackerFileName = @"_objectives\quests.json";
    private string _trackerPath = @"_objectives\";

    public async Task<IList<TrackingGroupViewModel>> ReadTrackers()
    {
        var trackers = new List<TrackingGroupViewModel>();

        var jsonOptions = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };

        var questsContent = await File.ReadAllTextAsync(_trackerFileName);
        var quests = JsonSerializer.Deserialize<List<QuestHeaderModel>>(questsContent, jsonOptions);

        if (quests is null)
            return trackers;

        foreach (var quest in quests)
        {
            var questPath = Path.Combine(_trackerPath, quest.FileName);
            var questContent = await File.ReadAllTextAsync(questPath);

            if (quest.Kind == QuestKind.Boss)
            {

            }
            else if (quest.Kind == QuestKind.Kill)
            {
                var killQuests = JsonSerializer.Deserialize<List<KillQuestModel>>(questContent, jsonOptions);

                if (killQuests is null)
                    continue;

                var trackingVm = new KillQuestTrackingViewModel()
                {
                    Name = quest.Name,
                    Quests = new(killQuests.Select(x => x.ToViewModel()))
                };

                trackers.Add(trackingVm);
            }
        }

        return trackers;
    }
}
