using System;
using System.IO;

namespace EHR_FileImportHelper.Services
{
    public sealed class ErgDirectoryMonitor : IDirectoryMonitor
    {
        private FileSystemWatcher? _watcher;

        public event EventHandler<string>? FileCreated;

        public void Start(string path)
        {
            Console.WriteLine("[ERG Monitor] Watching: " + path);

            if (!Directory.Exists(path)) return;

            _watcher = new FileSystemWatcher(path)
            {
                EnableRaisingEvents = true,
                IncludeSubdirectories = false,
                NotifyFilter = NotifyFilters.FileName
            };

            _watcher.Created += (_, e) =>
            {
                Console.WriteLine("[ERG Monitor] File created: " + e.FullPath);
                FileCreated?.Invoke(this, e.FullPath);
            };
        }

        public void Stop()
        {
            Console.WriteLine("[ERG Monitor] Stopping watcher");
            if (_watcher is not null)
            {
                _watcher.EnableRaisingEvents = false;
                _watcher.Dispose();
                _watcher = null;
            }
        }

        public void Dispose() => Stop();
    }
}
