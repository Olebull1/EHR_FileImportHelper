using System;

namespace EHR_FileImportHelper.Services
{
    public interface IDirectoryMonitor : IDisposable
    {
        event EventHandler<string> FileCreated;
        void Start(string directory);
        void Stop();
    }
}