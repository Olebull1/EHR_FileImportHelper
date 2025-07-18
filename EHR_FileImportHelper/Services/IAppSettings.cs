﻿namespace EHR_FileImportHelper.Services
{
    public interface IAppSettings
    {
        string SourceDirectory { get; set; }
        string DestinationDirectory { get; set; }
        bool IsErg {  get; set; }
        void Save();
    }
}