using EHR_FileImportHelper.Helpers;
using EHR_FileImportHelper.Services;
using System.Windows.Forms;
using System.Windows;
using Microsoft.Win32;
using System.Diagnostics.Eventing.Reader;

namespace EHR_FileImportHelper.ViewModels
{
    public sealed class SettingsViewModel : ObservableObject
    {
        private readonly IAppSettings _settings;
        private readonly Window _window;

        public SettingsViewModel(IAppSettings settings, Window window)
        {
            _settings = settings;
            _window = window;

            _sourceDirectory = settings.SourceDirectory;
            _destinationDirectory = settings.DestinationDirectory;
            _isErg = settings.IsErg;

            BrowseSourceCommand = new RelayCommand(BrowseSource);
            BrowseDestinationCommand = new RelayCommand(BrowseDestination);
            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(Cancel);
        }

        private string _sourceDirectory;
        public string SourceDirectory
        {
            get => _sourceDirectory;
            set => SetProperty(ref _sourceDirectory, value);
        }

        private string _destinationDirectory;
        public string DestinationDirectory
        {
            get => _destinationDirectory;
            set => SetProperty(ref _destinationDirectory, value);
        }
        private bool _isErg;
        public bool IsErg
        {
            get => _isErg;
            set => SetProperty(ref _isErg, value);
        }
        public RelayCommand BrowseSourceCommand { get; }
        public RelayCommand BrowseDestinationCommand { get; }
        public RelayCommand SaveCommand { get; }
        public RelayCommand CancelCommand { get; }

        private void BrowseSource()
        {
            var dlg = new System.Windows.Forms.FolderBrowserDialog { SelectedPath = SourceDirectory };
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                SourceDirectory = dlg.SelectedPath;
        }

        private void BrowseDestination()
        {
            var dlg = new System.Windows.Forms.FolderBrowserDialog { SelectedPath = DestinationDirectory };
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                DestinationDirectory = dlg.SelectedPath;
        }

        private void Save()
        {
            _settings.SourceDirectory = SourceDirectory;
            _settings.DestinationDirectory = DestinationDirectory;
            _settings.IsErg = IsErg;
            _settings.Save();
            _window.DialogResult = true;
            _window.Close();
        }
        private void Cancel()
        {
            _window.DialogResult = false;
            _window.Close();
        }
    }
}