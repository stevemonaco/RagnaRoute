using System.Collections.ObjectModel;
using NodaTime;
using CommunityToolkit.Mvvm.ComponentModel;

namespace RagnaRoute.ViewModels;
public partial class TrackerProfileViewModel : ViewModelBase
{
    public string ProfileName { get; }

    public ObservableCollection<TrackingGroupViewModel> TrackingGroups { get; } = new();
    [ObservableProperty] private TrackingGroupViewModel? _selectedTracker;
    [ObservableProperty] private string _trackerTime;

    private DateTimeZone? _timeZone;
    private ZonedClock? _clock;

    public TrackerProfileViewModel(string profileName, DateTimeZone? timeZone)
    {
        ProfileName = profileName;
        _timeZone = timeZone;

        if (_timeZone is not null)
            _clock = new ZonedClock(SystemClock.Instance, _timeZone, CalendarSystem.Julian);
    }

    public void UpdateTime()
    {
        if (_clock is not null)
        {
            var time = _clock.GetCurrentLocalDateTime();
            TrackerTime = $"{time.Hour}:{time.Minute:D2}:{time.Second:D2}";
        }
    }

    public void UpdateObjectives()
    {
        foreach (var tracker in TrackingGroups)
            tracker.UpdateObjective();
    }
}
