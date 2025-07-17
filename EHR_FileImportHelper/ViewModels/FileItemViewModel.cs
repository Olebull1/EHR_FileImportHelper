using System;
using System.IO;

namespace EHR_FileImportHelper.ViewModels
{
    public sealed class FileItemViewModel : ObservableObject
    {
        public string Name { get; }
        public string FullPath { get; }
        public DateTime Created { get; }

        public FileItemViewModel(string fullPath)
        {
            FullPath = fullPath;
            Name = Path.GetFileName(fullPath);
            Created = File.GetCreationTime(fullPath);
        }
    }
}