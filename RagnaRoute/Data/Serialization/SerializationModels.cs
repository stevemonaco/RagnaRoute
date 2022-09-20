using System.Collections.Generic;
using Cronos;
using NodaTime;

namespace RagnaRoute.Data;

public enum QuestKind { Boss, Scheduled, Instance }
public enum NotificationLevel { Silent, Low, High, Urgent }

public record TrackerProfileModel(string Name, DateTimeZone? TimeZone, List<TrackerGroupModel> QuestGroups);

public record TrackerGroupModel(string Name, string FileName, QuestKind Kind);
public record ScheduledQuestModel(string Name, string Description, List<string> Information, DateTimeZone TimeZone, CronExpression Repeat, int Duration);
public record BossQuestModel(string ObjectiveName, QuestKind ObjectiveType, int MinimumRespawn, int MaximumRespawn, NotificationLevel Notification, string WarpLocation, int? MobId);

public record MonsterModel(string Name, int Id, long HP, int BaseExp, int JobExp, MonsterElement Element, MonsterRace Race, MonsterSize Size);
