using RagnaRoute.Model;

namespace RagnaRoute.Data;
public class MonsterModel
{
    public string Name { get; set; }
    public int Id { get; set; }
    public long HP { get; set; }
    public int BaseExp { get; set; }
    public int JobExp { get; set; }
    public MonsterElement Element { get; set; }
    public MonsterRace Race { get; set; }
    public MonsterSize Size { get; set; }
}
