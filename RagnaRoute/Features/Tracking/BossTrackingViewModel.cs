using System.Collections.ObjectModel;
using System.Linq;
using ReactiveUI;
using RagnaRoute.Data;

namespace RagnaRoute.ViewModels;
public class BossTrackingViewModel : TrackingGroupViewModel
{
    private ObservableCollection<BossViewModel> _bosses = new();
    public ObservableCollection<BossViewModel> Bosses
    {
        get => _bosses;
        set => this.RaiseAndSetIfChanged(ref _bosses, value);
    }

    public BossTrackingViewModel(MonsterStore monsterStore)
    {
        _bosses = new(monsterStore.Monsters.Select(x => new BossViewModel(x.Name, x.HP, x.Element, x.Race, x.Size)));

        Name = "MVPs";
    }

    public override void UpdateObjective()
    {
        foreach (var boss in Bosses)
            boss.UpdateObjective();
    }
}
