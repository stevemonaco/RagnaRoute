using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using RagnaRoute.Services;
using CommunityToolkit.Mvvm.ComponentModel;

namespace RagnaRoute.ViewModels;
public partial class ShellViewModel : ViewModelBase
{
    private const string _profileFileName = @"_objectives/profile.json";

    [ObservableProperty] private TrackerProfileViewModel? _trackerProfile;

    [ObservableProperty] private ViewModelBase _selectedMenuItem = null!;
    [ObservableProperty] private ObservableCollection<ViewModelBase> _menuItems = null!;

    private readonly TrackerService _trackerService;
    private readonly QuestHistoryViewModel _questHistoryViewModel;

    public ShellViewModel(TrackerService trackerService, QuestHistoryViewModel questHistoryViewModel)
    {
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
