using Avalonia;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;

namespace RagnaRoute;
internal class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        try
        {
            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Unhandled exception bubbled up to the root level");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }


    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace();

    // EF Core uses this method at design time to access the DbContext
    public static IHostBuilder CreateHostBuilder(string[] args)
        => Host.CreateDefaultBuilder(args)
        .ConfigureServices(services =>
        {
            var bootstrapper = new Bootstrapper();
            bootstrapper.ConfigureServices(services);
            bootstrapper.ConfigureDbContext(services);
        });
}
