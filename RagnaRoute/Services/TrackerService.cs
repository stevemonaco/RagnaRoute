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
    private readonly MonsterStore _monsterStore;
    private string _trackerFileName = @"_objectives\quests.json";
    private string _trackerPath = @"_objectives\";

    public TrackerService(MonsterStore monsterStore)
    {
        _monsterStore = monsterStore;
    }

    public async Task<IList<TrackingGroupViewModel>> ReadTrackers()
    {
        var trackers = new List<TrackingGroupViewModel>();

        var jsonOptions = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };

        var questsContent = await File.ReadAllTextAsync(_trackerFileName);
        var questGroups = JsonSerializer.Deserialize<List<QuestGroupHeaderModel>>(questsContent, jsonOptions);

        if (questGroups is null)
            return trackers;

        foreach (var questGroup in questGroups)
        {
            var groupPath = Path.Combine(_trackerPath, questGroup.FileName);
            var groupContent = await File.ReadAllTextAsync(groupPath);

            if (questGroup.Kind == QuestKind.Boss)
            {
                var bossQuests = JsonSerializer.Deserialize<List<BossQuestModel>>(groupContent, jsonOptions);

                if (bossQuests is null)
                    continue;

                var bossQuestViewModels = bossQuests
                    .Select(x => x.MobId is int ? x.ToViewModel(_monsterStore.Monsters.First(y => y.Id == x.MobId))
                        : x.ToViewModel());

                var trackingVm = new BossQuestTrackingViewModel(bossQuestViewModels)
                {
                    Name = questGroup.Name
                };

                trackers.Add(trackingVm);
            }
            else if (questGroup.Kind == QuestKind.Kill)
            {
                var killQuests = JsonSerializer.Deserialize<List<KillQuestModel>>(groupContent, jsonOptions);

                if (killQuests is null)
                    continue;

                var trackingVm = new KillQuestTrackingViewModel()
                {
                    Name = questGroup.Name,
                    Quests = new(killQuests.Select(x => x.ToViewModel()))
                };

                trackers.Add(trackingVm);
            }
        }

        return trackers;
    }
}
