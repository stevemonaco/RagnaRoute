using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RagnaRoute.ViewModels;

public class KillQuestTrackingViewModel : TrackingGroupViewModel
{
    private ObservableCollection<KillQuestViewModel> _quests = new();
    public ObservableCollection<KillQuestViewModel> Quests
    {
        get => _quests;
        set => this.RaiseAndSetIfChanged(ref _quests, value);
    }

    public KillQuestTrackingViewModel()
    {
        _quests = new ObservableCollection<KillQuestViewModel>()
        {
            new("Eddga"),
            new("Garm"),
            new("Baphomet"),
            new("Mistress")
        };

        Name = "Kill Quest";
    }

    public override void UpdateObjective()
    {
        foreach (var quest in Quests)
            quest.UpdateObjective();
    }
}
