using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using NodaTime;
using RagnaRoute.ViewExtenders;
using RagnaRoute.Data;
using RagnaRoute.ViewModels;

namespace RagnaRoute.Services;
public class TrackerService
{
    private readonly MonsterStore _monsterStore;
    private readonly ISchedulerProvider _scheduler;
    private readonly CompletionService _completionService;
    private string _trackerPath = @"_objectives\";

    public TrackerService(MonsterStore monsterStore, ISchedulerProvider scheduler, CompletionService completionService)
    {
        _monsterStore = monsterStore;
        _scheduler = scheduler;
        _completionService = completionService;
    }

    public async Task<TrackerProfileViewModel> ReadTrackerProfile(string profileFileName)
    {
        var jsonOptions = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true,
            Converters =
            {
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase), new DateTimeZoneConverter(), new CronExpressionConverter()
            }
        };

        var profileContent = await File.ReadAllTextAsync(profileFileName).ConfigureAwait(false);
        var profile = JsonSerializer.Deserialize<TrackerProfileModel>(profileContent, jsonOptions);

        if (profile is null)
            throw new InvalidDataException($"Could not parse {profileFileName}");

        var profileVm = new TrackerProfileViewModel(profile.Name, profile.TimeZone);

        foreach (var questGroup in profile.QuestGroups)
        {
            var groupPath = Path.Combine(_trackerPath, questGroup.FileName);
            var groupContent = await File.ReadAllTextAsync(groupPath).ConfigureAwait(false);

            TrackingGroupViewModel? trackingVm = questGroup.Kind switch
            {
                QuestKind.Boss => await ReadBossQuestGroup(questGroup, groupContent, jsonOptions).ConfigureAwait(false),
                QuestKind.Scheduled => await ReadScheduledQuestGroup(questGroup, groupContent, jsonOptions).ConfigureAwait(false),
                _ => throw new NotSupportedException()
            };

            if (trackingVm is not null)
                profileVm.TrackingGroups.Add(trackingVm);
        }
        profileVm.SelectedTracker = profileVm.TrackingGroups.FirstOrDefault();

        return profileVm;
    }

    private async Task<BossQuestTrackingViewModel?> ReadBossQuestGroup(TrackerGroupModel group, string jsonContent, JsonSerializerOptions options)
    {
        var states = await _completionService.GetObjectives(group.Name, false);
        var stateMap = states.ToDictionary(x => x.ObjectiveName, x => x);

        var bossQuests = JsonSerializer.Deserialize<List<BossQuestModel>>(jsonContent, options);

        if (bossQuests is null)
            return null;

        var viewModels = new List<BossQuestViewModel>();

        foreach (var quest in bossQuests)
        {
            stateMap.TryGetValue(quest.ObjectiveName, out var state);
            var monsterModel = quest.MobId.HasValue ? _monsterStore.Monsters.First(x => x.Id == quest.MobId) : null;

            var viewModel = quest.ToViewModel(monsterModel, state);
            viewModels.Add(viewModel);
        }

        return new BossQuestTrackingViewModel(viewModels, _scheduler, _completionService)
        {
            Name = group.Name,
            DisplayName = group.Name
        };
    }

    private async Task<ScheduledQuestTrackingViewModel?> ReadScheduledQuestGroup(TrackerGroupModel group, string jsonContent, JsonSerializerOptions options)
    {
        var states = await _completionService.GetObjectives(group.Name, false);
        var stateMap = states.ToDictionary(x => x.ObjectiveName, x => x);

        var scheduledQuests = JsonSerializer.Deserialize<List<ScheduledQuestModel>>(jsonContent, options);

        if (scheduledQuests is null)
            return null;

        var viewModels = new List<ScheduledQuestViewModel>();

        foreach (var quest in scheduledQuests)
        {
            stateMap.TryGetValue(quest.ObjectiveName, out var state);

            var viewModel = quest.ToViewModel(state);
            viewModels.Add(viewModel);
        }

        return new ScheduledQuestTrackingViewModel(viewModels, _scheduler, _completionService)
        {
            Name = group.Name,
            DisplayName = group.Name
        };
    }
}
