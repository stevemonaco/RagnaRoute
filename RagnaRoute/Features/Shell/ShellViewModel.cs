using RagnaRoute.Data;
using RagnaRoute.Services;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace RagnaRoute.ViewModels;
public class ShellViewModel : ViewModelBase
{
    private ObservableCollection<TrackingGroupViewModel> _trackers;
    public ObservableCollection<TrackingGroupViewModel> Trackers
    {
        get => _trackers;
        set => this.RaiseAndSetIfChanged(ref _trackers, value);
    }

    private TrackingGroupViewModel? _selectedTracker;
    public TrackingGroupViewModel? SelectedTracker
    {
        get => _selectedTracker;
        set => this.RaiseAndSetIfChanged(ref _selectedTracker, value);
    }

    private readonly MonsterStore _monsterStore;
    private readonly TrackerService _trackerService;

    public ShellViewModel(MonsterStore monsterStore, TrackerService trackerService)
    {
        _monsterStore = monsterStore;
        _trackerService = trackerService;
    }

    public async Task InitializeTrackers()
    {
        var trackers = await _trackerService.ReadTrackers();
        Trackers = new(trackers);
        SelectedTracker = Trackers.FirstOrDefault();
    }

    public void UpdateObjectives()
    {
        foreach (var tracker in Trackers)
            tracker.UpdateObjective();
    }
}
