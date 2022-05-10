using Avalonia.Threading;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;

namespace RagnaRoute.ViewModels;
public class ShellViewModel : ViewModelBase
{
    private ObservableCollection<TrackingViewModel> _trackers;
    public ObservableCollection<TrackingViewModel> Trackers
    {
        get => _trackers;
        set => this.RaiseAndSetIfChanged(ref _trackers, value);
    }

    private readonly DispatcherTimer _timer;

    public ShellViewModel()
    {
        _trackers = new()
        {
            new BossTrackingViewModel(),
            new BossTrackingViewModel(),
            new BossTrackingViewModel()
        };

        _timer = new DispatcherTimer(TimeSpan.FromSeconds(1), DispatcherPriority.Normal, _timer_Tick);
        _timer.Start();
    }

    private void _timer_Tick(object? sender, EventArgs e)
    {
        foreach (var tracker in Trackers)
            tracker.UpdateObjective();
    }
}
