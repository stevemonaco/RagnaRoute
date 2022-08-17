using System.Threading.Tasks;
using RagnaRoute.Data;
using RagnaRoute.Services;
using CommunityToolkit.Mvvm.ComponentModel;

namespace RagnaRoute.ViewModels;
public partial class ShellViewModel : ViewModelBase
{
    private const string _profileFileName = @"_objectives/profile.json";

    [ObservableProperty] private TrackerProfileViewModel? _trackerProfile;

    private readonly MonsterStore _monsterStore;
    private readonly TrackerService _trackerService;

    public ShellViewModel(MonsterStore monsterStore, TrackerService trackerService)
    {
        _monsterStore = monsterStore;
        _trackerService = trackerService;
    }

    public async Task InitializeTrackers()
    {
        TrackerProfile = await _trackerService.ReadTrackerProfile(_profileFileName);
    }

    public void UpdateObjectives()
    {
        TrackerProfile?.UpdateObjectives();
    }
}
