using At.luki0606.ClassPulse.Data;
using System.IO;
using System.Text.Json;

namespace At.luki0606.ClassPulse.Services
{
    public class SettingsService
    {
        private readonly string _settingsFilePath;
        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            WriteIndented = true
        };

        public SettingsService()
        {
            string appdataFoldetrPath = Utils.GetAppdataFolderPath();
            _settingsFilePath = Path.Combine(appdataFoldetrPath, "settings.json");
        }

        public AppSettings LoadSettings()
        {
            if (!File.Exists(_settingsFilePath))
            {
                return new AppSettings();
            }

            try
            {
                string json = File.ReadAllText(_settingsFilePath);
                return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();

            }
            catch
            {
                return new AppSettings();
            }
        }

        public void SaveSettings(AppSettings settings)
        {
            try
            {
                string json = JsonSerializer.Serialize(settings, _jsonOptions);
                File.WriteAllText(_settingsFilePath, json);
            }
            catch
            {
                // Handle exceptions if needed
            }
        }
    }
}
