using Avalonia.Threading;
using System.Reactive.Concurrency;

namespace RagnaRoute.ViewExtenders;

public interface ISchedulerProvider
{
    IScheduler Background { get; }
    IScheduler Main { get; }
}

public class SchedulerProvider : ISchedulerProvider
{
    public IScheduler Main { get; }
    public IScheduler Background { get; }

    public SchedulerProvider()
    {
        Background = TaskPoolScheduler.Default;
        Main = AvaloniaScheduler.Instance;
    }
}
