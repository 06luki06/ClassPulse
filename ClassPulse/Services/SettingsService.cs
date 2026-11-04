using At.luki0606.ClassPulse.Data;
using System.IO;
using System.Text.Json;

namespace At.luki0606.ClassPulse.Services
{
    /// <summary>
    /// Reads and writes <see cref="AppSettings"/> as a JSON file.
    /// </summary>
    public class SettingsService
    {
        private readonly string _settingsFilePath;
        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            WriteIndented = true
        };

        /// <summary>
        /// Initializes a new instance that uses <c>settings.json</c> in the application data folder.
        /// </summary>
        public SettingsService() : this(Path.Combine(Utils.GetAppdataFolderPath(), "settings.json"))
        {
        }

        /// <summary>
        /// Initializes a new instance that uses the given file.
        /// </summary>
        /// <param name="settingsFilePath">The path of the settings file.</param>
        public SettingsService(string settingsFilePath)
        {
            _settingsFilePath = settingsFilePath;
        }

        /// <summary>
        /// Loads the settings.
        /// </summary>
        /// <returns>The stored settings, or the defaults if the file is missing or cannot be read.</returns>
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

        /// <summary>
        /// Saves the settings. Failures are ignored.
        /// </summary>
        /// <param name="settings">The settings to save.</param>
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
