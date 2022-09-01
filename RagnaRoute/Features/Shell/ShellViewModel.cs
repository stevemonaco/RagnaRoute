using System.Threading.Tasks;
using RagnaRoute.Data;
using RagnaRoute.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;

namespace RagnaRoute.ViewModels;
public partial class ShellViewModel : ViewModelBase
{
    private const string _profileFileName = @"_objectives/profile.json";

    [ObservableProperty] private TrackerProfileViewModel? _trackerProfile;

    [ObservableProperty] private ViewModelBase _selectedMenuItem;
    [ObservableProperty] private ObservableCollection<ViewModelBase> _menuItems;

    private readonly MonsterStore _monsterStore;
    private readonly TrackerService _trackerService;
    private readonly QuestHistoryViewModel _questHistoryViewModel;

    public ShellViewModel(MonsterStore monsterStore, TrackerService trackerService, QuestHistoryViewModel questHistoryViewModel)
    {
        _monsterStore = monsterStore;
        _trackerService = trackerService;
        _questHistoryViewModel = questHistoryViewModel;
    }

    public async Task InitializeTrackers()
    {
        TrackerProfile = await _trackerService.ReadTrackerProfile(_profileFileName);

        var items = new ObservableCollection<ViewModelBase>(TrackerProfile.TrackingGroups);
        items.Add(_questHistoryViewModel);
        MenuItems = items;
    }

    public void UpdateObjectives()
    {
        TrackerProfile?.UpdateTime();

        foreach (var tracker in MenuItems.OfType<TrackingGroupViewModel>())
            tracker.UpdateObjective();
    }
}
