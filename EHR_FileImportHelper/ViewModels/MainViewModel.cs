using EHR_FileImportHelper.Helpers;
using EHR_FileImportHelper.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace EHR_FileImportHelper.ViewModels
{
    public sealed class MainViewModel : ObservableObject
    {
        private readonly IDirectoryMonitor _monitor;
        private readonly IFileImporter _importer;
        private readonly IAppSettings _settings;
        private readonly Dispatcher _uiDispatcher = Dispatcher.CurrentDispatcher;
        private readonly System.IServiceProvider _sp;

        public MainViewModel(IDirectoryMonitor monitor,
                             IFileImporter importer,
                             IAppSettings settings,
                             System.IServiceProvider sp)
        {
            _monitor = monitor;
            _importer = importer;
            _settings = settings;
            _sp = sp;

            ImportCommand = new AsyncRelayCommand(ImportAsync, () => Selected != null);
            SettingsCommand = new RelayCommand(OpenSettings);

            StartMonitoring();
        }

        // ───────────────────────── UI‑bound properties ─────────────────────────

        public ObservableCollection<FileItemViewModel> Files { get; } = new();

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

        public string SourceDirectory => _settings.SourceDirectory;
        public string DestinationDirectory => _settings.DestinationDirectory;

        public AsyncRelayCommand ImportCommand { get; }
        public RelayCommand SettingsCommand { get; }

        // ───────────────────────── Logic ─────────────────────────

        private void StartMonitoring()
        {
            _monitor.Stop();
            _monitor.FileCreated -= OnFileCreated;

            Files.Clear();

            // Only proceed if both paths are set and exist
            if (string.IsNullOrWhiteSpace(_settings.SourceDirectory) ||
                string.IsNullOrWhiteSpace(_settings.DestinationDirectory))
            {
                return; // Skip startup logic
            }

            if (!Directory.Exists(_settings.SourceDirectory))
                return;

            foreach (var path in Directory.GetFiles(_settings.SourceDirectory))
                Files.Add(new FileItemViewModel(path));

            _monitor.FileCreated += OnFileCreated;
            _monitor.Start(_settings.SourceDirectory);

            OnPropertyChanged(nameof(SourceDirectory));
            OnPropertyChanged(nameof(DestinationDirectory));
        }

        private void OnFileCreated(object? _, string path)
            => _uiDispatcher.Invoke(() => Files.Add(new FileItemViewModel(path)));

        private async Task ImportAsync()
        {
            if (Selected is null) return;
            await _importer.ImportAsync(Selected.FullPath, _settings.DestinationDirectory);
            Files.Remove(Selected);
        }

        private void OpenSettings()
        {
            var win = _sp.GetRequiredService<Views.SettingsWindow>();
            win.Owner = System.Windows.Application.Current.MainWindow;
            if (win.ShowDialog() == true)
            {
                // reload paths & monitor
                StartMonitoring();
            }
        }
    }
}