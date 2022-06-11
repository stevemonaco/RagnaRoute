using RagnaRoute.Features.Data;
using System.Collections.Generic;

namespace RagnaRoute.Data;

public enum QuestKind { Boss, Kill }

public record QuestHeaderModel(string Name, string FileName, QuestKind Kind);
public record KillQuestModel(string Name, string Description, List<string> Information);

public record MonsterModel(string Name, int Id, long HP, int BaseExp, int JobExp, MonsterElement Element, MonsterRace Race, MonsterSize Size);
