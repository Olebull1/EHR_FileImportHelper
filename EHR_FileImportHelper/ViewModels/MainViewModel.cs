using EHR_FileImportHelper.Helpers;
using EHR_FileImportHelper.Services;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace EHR_FileImportHelper.ViewModels
{
    public sealed class MainViewModel : ObservableObject
    {
        private readonly Func<IDirectoryMonitor> _monitorFactory;
        private IDirectoryMonitor _monitor;
        private readonly IFileImporter _importer;
        private readonly IAppSettings _settings;
        private readonly Func<Window> _settingsWindowFactory;
        private readonly Dispatcher _uiDispatcher = Dispatcher.CurrentDispatcher;

        public ObservableCollection<FileItemViewModel> Files { get; } = [];

        private FileItemViewModel? _selected;
        public FileItemViewModel? Selected
        {
            get => _selected;
            set
            {
                if (SetProperty(ref _selected, value))
                    ImportCommand.RaiseCanExecuteChanged();
            }
        }

        public AsyncRelayCommand ImportCommand { get; }
        public RelayCommand SettingsCommand { get; }

        public string SourceDirectory => _settings.SourceDirectory;
        public string DestinationDirectory => _settings.DestinationDirectory;

        public MainViewModel(Func<IDirectoryMonitor> monitorFactory,
                             IFileImporter importer,
                             IAppSettings settings,
                             Func<Window> settingsWindowFactory)
        {
            _monitorFactory = monitorFactory;
            _importer = importer;
            _settings = settings;
            _settingsWindowFactory = settingsWindowFactory;

            _monitor = _monitorFactory();

            ImportCommand = new AsyncRelayCommand(ImportAsync, CanImport);
            SettingsCommand = new RelayCommand(OpenSettings);

            if (IsConfigured)
                StartMonitoring();
        }

        private bool IsConfigured =>
            Directory.Exists(_settings.SourceDirectory) &&
            Directory.Exists(_settings.DestinationDirectory);

        private void StartMonitoring()
        {
            _monitor.Stop();
            _monitor.FileCreated -= OnFileCreated;

            Files.Clear();

            if (!IsConfigured)
                return;

            foreach (var path in Directory.GetFiles(_settings.SourceDirectory))
                Files.Add(new FileItemViewModel(path));

            _monitor.FileCreated += OnFileCreated;
            _monitor.Start(_settings.SourceDirectory);

            OnPropertyChanged(nameof(SourceDirectory));
            OnPropertyChanged(nameof(DestinationDirectory));
        }

        private void OnFileCreated(object? sender, string fullPath)
        {
            _uiDispatcher.Invoke(() => Files.Add(new FileItemViewModel(fullPath)));
        }

        private async Task ImportAsync()
        {
            if (Selected is null) return;

            await _importer.ImportAsync(Selected.FullPath, _settings.DestinationDirectory);
            Files.Remove(Selected);
        }

        private bool CanImport()
        {
            return Selected != null &&
                   Directory.Exists(_settings.SourceDirectory) &&
                   Directory.Exists(_settings.DestinationDirectory);
        }

        private void OpenSettings()
        {
            var window = _settingsWindowFactory();
            window.Owner = System.Windows.Application.Current.MainWindow;

            if (window.ShowDialog() == true)
            {
                _monitor.Stop();
                _monitor = _monitorFactory();
                StartMonitoring();
                ImportCommand.RaiseCanExecuteChanged();
            }
        }
    }
}
