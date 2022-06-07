namespace RagnaRoute.Features.Data;

public enum QuestKind { Boss, Kill }

public class QuestHeaderModel
{
    public string Name { get; set; }
    public string FileName { get; set; }
    public QuestKind Kind { get; set; }
}
