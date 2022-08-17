using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace RagnaRoute.ViewModels;

public partial class KillQuestTrackingViewModel : TrackingGroupViewModel
{
    [ObservableProperty] private ObservableCollection<KillQuestViewModel> _quests = new();
    //private readonly IClipboardService _clipboardService;

    //public ReactiveCommand<string, Unit> CopyObjectiveInformationCommand { get; }

    public KillQuestTrackingViewModel()
    {
        //_clipboardService = clipboardService;

        //CopyObjectiveInformationCommand = ReactiveCommand.Create<string>(CopyObjectiveInformation);
    }

    public override void UpdateObjective()
    {
        foreach (var quest in Quests)
            quest.UpdateObjective();
    }

    //public async void CopyObjectiveInformation(string information)
    //{
    //    var result = await _clipboardService.CopyTextAsync(information);
    //}
}
