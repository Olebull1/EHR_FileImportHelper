using System;
using System.IO;

namespace EHR_FileImportHelper.ViewModels
{
    public sealed class FileItemViewModel(string fullPath) : ObservableObject
    {
        public string Name { get; } = Path.GetFileName(fullPath);
        public string FullPath { get; } = fullPath;
        public DateTime Created { get; } = File.GetCreationTime(fullPath);
    }
}