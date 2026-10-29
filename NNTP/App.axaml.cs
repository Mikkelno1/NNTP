using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using AvaloniaExercises.Views;
using NNTP.ViewModels;
using NNTP.Views;
using Microsoft.Extensions.DependencyInjection;
using NNTP.Models;
using NNTP.Services;
using NNTP.ViewModels;
using NNTP.Views;

namespace NNTP;

public partial class App : Application
{
    //Exposes App for use in other classes
    public new static App Current => (App)Application.Current;
    
    //Exposes IServiceProvider and saves the reference to the DI-container, which is IServiceProvider
    public IServiceProvider Services { get; set; }
    
    /**
     * Builds DI-container on startup
     */
    public override void Initialize()
    {
        //Creates and configures services and ViewModels inside the DI-container
        Services = ConfigureServices();
        
        //Loads the XAML-files
        AvaloniaXamlLoader.Load(this);
    }

    /**
     * Runs once the avalonia framework is ready to create a UI window
     */
    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            //Gets the unique instance of MainViewModel (singleton) from the DI-container
            var mainViewModel = Services.GetRequiredService<MainViewModel>();

            //Setting the start page to LoginView
            mainViewModel.CurrentPage = Services.GetRequiredService<LoginViewModel>();
            
            //Creating MainWindow and sets mainViewModel as its datacontext
            desktop.MainWindow = new MainWindow
            {
                DataContext = mainViewModel
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    /**
     * Registers all services and ViewModels inside the DI-container
     */
    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        //Singletons are primarily used for navigation to windows where the window instance should always be the same
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<INewsGroupService, NewsGroupService>();
        services.AddSingleton<IConnection, Connection>();
        services.AddSingleton<MainViewModel>();
        
        //Transients are primarily used for navigation to windows where the window needs to return to a default state
        //A good example is a login window - we don't want login details to be shown if we change views back to the
        //login view, as the state would be saved with a singleton and not reset as with a transient
        services.AddTransient<ArticleViewModel>();
        services.AddTransient<LoginViewModel>();
        
        
        
        

        //Builds and returns our DI-container
        return services.BuildServiceProvider();
    }
}