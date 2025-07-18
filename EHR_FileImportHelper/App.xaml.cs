using EHR_FileImportHelper.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;

namespace EHR_FileImportHelper
{
    public partial class App : System.Windows.Application
    {
        public static IServiceProvider Services { get; private set; } = default!;

        protected override void OnStartup(StartupEventArgs e)
        {
            var services = new ServiceCollection();

            // Register app-level services
            services.AddSingleton<IAppSettings, JsonAppSettings>();
            services.AddTransient<IFileImporter, FileImporter>();

            // Register both monitors
            services.AddTransient<DirectoryMonitor>();
            services.AddTransient<ErgDirectoryMonitor>();

            // Register monitor selector based on ERG flag
            services.AddSingleton<Func<IDirectoryMonitor>>(sp => () =>
            {
                var settings = sp.GetRequiredService<IAppSettings>();
                return settings.IsErg
                    ? sp.GetRequiredService<ErgDirectoryMonitor>() as IDirectoryMonitor
                    : sp.GetRequiredService<DirectoryMonitor>();
            });

            // Settings window factory
            //services.AddTransient<Views.SettingsWindow>();
            //services.AddSingleton<Func<Views.SettingsWindow>>(sp => () => sp.GetRequiredService<Views.SettingsWindow>());
            services.AddTransient<Views.SettingsWindow>();
            services.AddSingleton<Func<Window>>(sp => () => sp.GetRequiredService<Views.SettingsWindow>());

            // Register MainViewModel and MainWindow
            services.AddSingleton<ViewModels.MainViewModel>();
            var provider = services.BuildServiceProvider();
            Services = provider;

            var mainWindow = new Views.MainWindow
            {
                DataContext = provider.GetRequiredService<ViewModels.MainViewModel>()
            };

            mainWindow.Show();
        }
    }
}
