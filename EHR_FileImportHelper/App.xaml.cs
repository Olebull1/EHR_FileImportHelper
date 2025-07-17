using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using EHR_FileImportHelper.Services;

namespace EHR_FileImportHelper
{
    public partial class App : System.Windows.Application
    {
        public static IServiceProvider Services { get; private set; } = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var services = new ServiceCollection();

            // Singletons
            services.AddSingleton<IAppSettings, JsonAppSettings>();
            services.AddSingleton<ViewModels.MainViewModel>();
            services.AddSingleton<IDirectoryMonitor, DirectoryMonitor>();
            services.AddSingleton<IFileImporter, FileImporter>();

            // Transient Settings window so DI can inject IAppSettings
            services.AddTransient<Views.SettingsWindow>();

            Services = services.BuildServiceProvider();
        }
    }
}