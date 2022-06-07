using RagnaRoute.Data;
using ReactiveUI;
using System.Collections.ObjectModel;

namespace RagnaRoute.ViewModels;
public class ShellViewModel : ViewModelBase
{
    public ObservableCollection<TrackingGroupViewModel> Trackers { get; } = new();

    private TrackingGroupViewModel? _selectedTracker;
    public TrackingGroupViewModel? SelectedTracker
    {
        get => _selectedTracker;
        set => this.RaiseAndSetIfChanged(ref _selectedTracker, value);
    }

    public ShellViewModel(MonsterStore monsterStore)
    {
        //_trackers = new()
        //{
        //    new BossTrackingViewModel(monsterStore),
        //    new KillQuestTrackingViewModel(),
        //    new KillQuestTrackingViewModel()
        //};
    }

    public void UpdateObjectives()
    {
        foreach (var tracker in Trackers)
            tracker.UpdateObjective();
    }
}
