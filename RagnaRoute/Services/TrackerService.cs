using RagnaRoute.Data;
using RagnaRoute.ViewModels;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using NodaTime;
using RagnaRoute.ViewExtenders;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

namespace RagnaRoute.Services;
public class TrackerService
{
    private readonly MonsterStore _monsterStore;
    private readonly ISchedulerProvider _scheduler;
    private readonly QuestService _questService;
    private string _trackerPath = @"_objectives\";

    public TrackerService(MonsterStore monsterStore, ISchedulerProvider scheduler, QuestService questService)
    {
        _monsterStore = monsterStore;
        _scheduler = scheduler;
        _questService = questService;
    }

    public async Task<TrackerProfileViewModel> ReadTrackerProfile(string profileFileName)
    {
        var jsonOptions = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };

        var profileContent = await File.ReadAllTextAsync(profileFileName).ConfigureAwait(false);
        var profile = JsonSerializer.Deserialize<TrackerProfileModel>(profileContent, jsonOptions);

        if (profile is null)
            throw new InvalidDataException($"Could not parse {profileFileName}");
        
        var zone = DateTimeZoneProviders.Tzdb.GetZoneOrNull(profile.TimeZone ?? "");

        if (zone is null && profile.TimeZone is null)
            throw new InvalidDataException($"Could not parse time zone: {profile.TimeZone}");

        var profileVm = new TrackerProfileViewModel(profile.Name, zone);

        foreach (var questGroup in profile.QuestGroups)
        {
            var groupPath = Path.Combine(_trackerPath, questGroup.FileName);
            var groupContent = await File.ReadAllTextAsync(groupPath).ConfigureAwait(false);

            if (questGroup.Kind == QuestKind.Boss)
            {
                var bossQuests = JsonSerializer.Deserialize<List<BossQuestModel>>(groupContent, jsonOptions);

                if (bossQuests is null)
                    continue;

                var bossQuestViewModels = bossQuests
                    .Select(x => x.MobId is int 
                        ? x.ToViewModel(_monsterStore.Monsters.First(y => y.Id == x.MobId))
                        : x.ToViewModel());

                var trackingVm = new BossQuestTrackingViewModel(bossQuestViewModels, _scheduler, _questService)
                {
                    Name = questGroup.Name,
                    DisplayName = questGroup.Name
                };

                profileVm.TrackingGroups.Add(trackingVm);
            }
            else if (questGroup.Kind == QuestKind.Kill)
            {
                var killQuests = JsonSerializer.Deserialize<List<KillQuestModel>>(groupContent, jsonOptions);

                if (killQuests is null)
                    continue;

                var trackingVm = new KillQuestTrackingViewModel()
                {
                    Name = questGroup.Name,
                    DisplayName = questGroup.Name,
                    Quests = new(killQuests.Select(x => x.ToViewModel()))
                };

                profileVm.TrackingGroups.Add(trackingVm);
            }
        }
        profileVm.SelectedTracker = profileVm.TrackingGroups.FirstOrDefault();

        return profileVm;
    }
}
