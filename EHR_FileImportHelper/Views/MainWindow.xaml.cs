using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace EHR_FileImportHelper.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = EHR_FileImportHelper.App.Services.GetRequiredService<ViewModels.MainViewModel>();
        }
    }
}