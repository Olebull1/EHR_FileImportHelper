using System;
using System.IO;
using System.Text;
using System.Text.Json;

namespace EHR_FileImportHelper.Services
{
    public sealed class JsonAppSettings : IAppSettings
    {
        private const string FileName = "settings.json";
        private static readonly JsonSerializerOptions _opts = new(JsonSerializerDefaults.General) { WriteIndented = true };

        public string SourceDirectory { get; set; } = "";
        public string DestinationDirectory { get; set; } = "";
        public bool isErg { get; set; } = false;

        private readonly string _filePath;

        public JsonAppSettings()
        {
            _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, FileName);
            Load();
        }

        private void Load()
        {
            if (!File.Exists(_filePath)) return;

            try
            {
                var json = File.ReadAllText(_filePath);

                // deserialize into a simple DTO to avoid recursive construction
                var loaded = JsonSerializer.Deserialize<AppSettingsDto>(json, _opts);
                if (loaded is not null)
                {
                    SourceDirectory = loaded.SourceDirectory;
                    DestinationDirectory = loaded.DestinationDirectory;
                    isErg = loaded.isErg;
                }
            }
            catch
            {
                /* fall back to defaults */
            }
        }

        public void Save()
        {
            var json = JsonSerializer.Serialize(this, _opts);
            File.WriteAllText(_filePath, json);
        }
    }
    internal class AppSettingsDto
    {
        public string SourceDirectory { get; set; } = "C:/Watched";
        public string DestinationDirectory { get; set; } = "C:/Imported";
        public bool isErg { get; set; } = false;
    }
}