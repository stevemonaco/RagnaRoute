using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Splat;
using Splat.Microsoft.Extensions.DependencyInjection;
using RagnaRoute.ViewModels;
using RagnaRoute.Views;
using System;
using RagnaRoute.Data;

namespace RagnaRoute;
public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var services = new ServiceCollection();
        var bootstrapper = new Bootstrapper();
        bootstrapper.ConfigureIoc(services);
        bootstrapper.ConfigureServices(services);
        bootstrapper.ConfigureViews(services);
        bootstrapper.ConfigureViewModels(services);
        //await bootstrapper.LoadConfigurations(services);

        services.UseMicrosoftDependencyResolver();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var shellViewModel = Locator.Current.GetService<ShellViewModel>();
            var shellView = Locator.Current.GetService<ShellView>();
            shellView!.DataContext = shellViewModel;
            desktop.MainWindow = shellView;
        }

        base.OnFrameworkInitializationCompleted();
    }
}
