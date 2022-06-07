using RagnaRoute.Services;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive;

namespace RagnaRoute.ViewModels;

public class KillQuestTrackingViewModel : TrackingGroupViewModel
{
    private ObservableCollection<KillQuestViewModel> _quests = new();
    //private readonly IClipboardService _clipboardService;

    public ObservableCollection<KillQuestViewModel> Quests
    {
        get => _quests;
        set => this.RaiseAndSetIfChanged(ref _quests, value);
    }

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
