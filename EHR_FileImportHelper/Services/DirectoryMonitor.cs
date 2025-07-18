using System;
using System.IO;

namespace EHR_FileImportHelper.Services
{
    public sealed class DirectoryMonitor : IDirectoryMonitor
    {
        private FileSystemWatcher? _watcher;

        public event EventHandler<string>? FileCreated;

        public void Start(string directory)
        {
            Stop();
            Console.WriteLine("[DirectoryMonitor] Started");
            _watcher = new FileSystemWatcher(directory)
            {
                EnableRaisingEvents = true,
                IncludeSubdirectories = false,
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.CreationTime
            };

            _watcher.Created += OnCreated;
        }

        private void OnCreated(object sender, FileSystemEventArgs e)
            => FileCreated?.Invoke(this, e.FullPath);

        public void Stop()
        {
            if (_watcher != null)
            {
                _watcher.EnableRaisingEvents = false;
                _watcher.Created -= OnCreated;
                _watcher.Dispose();
                _watcher = null;
            }
        }

        public void Dispose() => Stop();
    }
}