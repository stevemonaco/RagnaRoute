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

        var bossQuestViewModels = bossQuests
            .Select(x => x.MobId is int
                ? x.ToViewModel(_monsterStore.Monsters.First(y => y.Id == x.MobId))
                : x.ToViewModel())
            .ToList();

        foreach (var bossQuestViewModel in bossQuestViewModels)
        {
            if (stateMap.TryGetValue(bossQuestViewModel.Name, out var state))
            {
                bossQuestViewModel.IsHidden = state.IsHidden;

                if (state.LastCompletion is Instant instant)
                {
                    bossQuestViewModel.UpdateObjective();
                    bossQuestViewModel.Objective.Recur(instant);
                    bossQuestViewModel.UpdateObjective();
                }
            }
        }

        return new BossQuestTrackingViewModel(bossQuestViewModels, _scheduler, _completionService)
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

        var scheduledQuestViewModels = scheduledQuests
            .Select(x => x.ToViewModel())
            .ToList();

        foreach (var scheduledQuestViewModel in scheduledQuestViewModels)
        {
            if (stateMap.TryGetValue(scheduledQuestViewModel.Name, out var state))
            {
                scheduledQuestViewModel.IsHidden = state.IsHidden;

                if (state.LastCompletion is Instant instant)
                {
                    scheduledQuestViewModel.UpdateObjective();
                    scheduledQuestViewModel.Objective.Reset();
                    scheduledQuestViewModel.UpdateObjective();
                }
            }
        }

        return new ScheduledQuestTrackingViewModel(_completionService)
        {
            Name = group.Name,
            DisplayName = group.Name,
            Quests = new(scheduledQuests.Select(x => x.ToViewModel()))
        };
    }
}
