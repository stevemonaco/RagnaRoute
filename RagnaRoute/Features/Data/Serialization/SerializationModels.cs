using System.Collections.Generic;

namespace RagnaRoute.Data;

public enum QuestKind { Boss, Kill, Instance }
public enum NotificationLevel { Silent, Low, High, Urgent }

public record TrackerProfileModel(string Name, string? TimeZone, List<TrackerGroupModel> QuestGroups);

public record TrackerGroupModel(string Name, string FileName, QuestKind Kind);
public record KillQuestModel(string Name, string Description, List<string> Information);
public record BossQuestModel(string ObjectiveName, QuestKind ObjectiveType, int MinimumRespawn, int MaximumRespawn, NotificationLevel Notification, string WarpLocation, int? MobId);

public record MonsterModel(string Name, int Id, long HP, int BaseExp, int JobExp, MonsterElement Element, MonsterRace Race, MonsterSize Size);
