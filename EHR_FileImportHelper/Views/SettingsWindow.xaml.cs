using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using EHR_FileImportHelper.Services;

namespace EHR_FileImportHelper.Views
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow(IAppSettings settings)
        {
            InitializeComponent();
            DataContext = new ViewModels.SettingsViewModel(settings, this);
        }
    }
}