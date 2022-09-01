using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using RagnaRoute.Data;
using RagnaRoute.Services;
using RagnaRoute.ViewExtenders;
using RagnaRoute.ViewModels;
using Serilog;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RagnaRoute;
public interface IAppBootstrapper<TViewModel> where TViewModel : class
{
    void ConfigureServices(IServiceCollection services);
    void ConfigureViews(IServiceCollection services);
    void ConfigureViewModels(IServiceCollection services);
    void ConfigureDbContext(IServiceCollection services);

    Task<bool> LoadConfigurations(IServiceProvider provider);
}

public class Bootstrapper : IAppBootstrapper<ShellViewModel>
{
    private LoggerFactory _loggerFactory;

    private const string _logFileName = @"log.txt";
    private const string _monsterDataFileName = @"_data/mob.csv";
    private const string _connectionString = @"Filename=./_objectives/Completions.sqlite";

    public void ConfigureIoc(IServiceCollection services)
    {
        _loggerFactory = CreateLoggerFactory(_logFileName);
    }

    public void ConfigureServices(IServiceCollection services)
    {
        var monsterStore = MonsterStore.LoadMonstersFromCsv(_monsterDataFileName);
        services.AddSingleton(monsterStore);
        services.AddTransient<TrackerService>();
        services.AddSingleton<ISchedulerProvider, SchedulerProvider>();
        services.AddTransient<QuestService>();
    }

    public void ConfigureViews(IServiceCollection services)
    {
        var viewTypes = GetType().Assembly.GetTypes().Where(x => x.Name.EndsWith("View"));

        foreach (var viewType in viewTypes)
            services.AddTransient(viewType);
    }

    public void ConfigureViewModels(IServiceCollection services)
    {
        services.TryAddSingleton<ShellViewModel>();
        services.TryAddSingleton<QuestHistoryViewModel>();

        var vmTypes = GetType()
            .Assembly
            .GetTypes()
            .Where(x => x.Name.EndsWith("ViewModel"))
            .Where(x => !x.IsAbstract && !x.IsInterface);

        foreach (var vmType in vmTypes)
            services.TryAddTransient(vmType);
    }

    public void ConfigureDbContext(IServiceCollection services)
    {
        services.AddDbContextFactory<RagnaContext>(
            options => options.UseSqlite(_connectionString, x => x.UseNodaTime())
            );
    }

    private LoggerFactory CreateLoggerFactory(string logName)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Error()
            .WriteTo.File(logName, rollingInterval: RollingInterval.Month,
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}{NewLine}")
            .CreateLogger();

        var factory = new LoggerFactory();
        factory.AddSerilog(Log.Logger);
        return factory;
    }

    public void EnsureDatabaseAvailable(ServiceProvider provider)
    {
        using var context = provider.GetService<RagnaContext>()!;

        //context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
    }

    public async Task<bool> LoadConfigurations(IServiceProvider provider)
    {
        var questHistory = provider.GetService<QuestHistoryViewModel>()!;
        await questHistory.InitializeProfiles();

        //var monsterStore = await MonsterStore.LoadMonstersFromCsvAsync(@"Assets/mob.csv");
        //services.AddSingleton(monsterStore);

        return true;
    }

    //protected override void OnUnhandledException(DispatcherUnhandledExceptionEventArgs e)
    //{
    //    base.OnUnhandledException(e);

    //    Log.Error(e.Exception, "Unhandled exception");

    //    if (!_isStarting)
    //    {
    //        _container?.Resolve<IWindowManager>()?.ShowMessageBox($"{e.Exception.Message}", "Unhandled Exception", MessageBoxButton.OK, MessageBoxImage.Error);
    //        e.Handled = true;
    //    }
    //}
}
