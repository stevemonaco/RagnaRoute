using Avalonia.Threading;
using System.Reactive.Concurrency;

namespace RagnaRoute.ViewExtenders;

public interface ISchedulerProvider
{
    /// <summary>
    /// Thread is to be performed on any background thread
    /// </summary>
    IScheduler Background { get; }

    /// <summary>
    /// Work is to be performed on a thread that supports UI
    /// </summary>
    IScheduler Visual { get; }
}

public class SchedulerProvider : ISchedulerProvider
{
    public IScheduler Visual { get; }
    public IScheduler Background { get; }

    public SchedulerProvider()
    {
        Background = TaskPoolScheduler.Default;
        Visual = AvaloniaScheduler.Instance;
    }
}
