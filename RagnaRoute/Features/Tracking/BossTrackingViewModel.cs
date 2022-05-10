using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RagnaRoute.Model;

namespace RagnaRoute.ViewModels;
public class BossTrackingViewModel : TrackingViewModel
{
    private ObservableCollection<BossViewModel> _bosses = new();
    public ObservableCollection<BossViewModel> Bosses
    {
        get => _bosses;
        set => this.RaiseAndSetIfChanged(ref _bosses, value);
    }

    public BossTrackingViewModel()
    {
        _bosses = new ObservableCollection<BossViewModel>()
        {
            new("Eddga", 100000, MonsterElement.Fire4, MonsterRace.Brute, MonsterSize.Large),
            new("Garm", 200000, MonsterElement.Water4, MonsterRace.Brute, MonsterSize.Large),
            new("Baphomet", 300000, MonsterElement.Shadow4, MonsterRace.Demon, MonsterSize.Large),
            new("Mistress", 150000, MonsterElement.Wind4, MonsterRace.Insect, MonsterSize.Small)
        };

        Name = "MVPs";
    }

    public override void UpdateObjective()
    {
        foreach (var boss in Bosses)
            boss.UpdateObjective();
    }
}
