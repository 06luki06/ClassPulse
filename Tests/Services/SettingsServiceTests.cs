using At.luki0606.ClassPulse.Data;
using At.luki0606.ClassPulse.Services;

namespace At.luki0606.ClassPulse.Tests.Services
{
    [TestFixture]
    internal class SettingsServiceTests
    {
        private string _tempFilePath;
        private SettingsService _service;

        [SetUp]
        public void Setup()
        {
            _tempFilePath = Path.Combine(Path.GetTempPath(), $"settings_test_{System.Guid.NewGuid()}.json");
            _service = new SettingsService(_tempFilePath);
        }

        [TearDown]
        public void TearDown()
        {
            if (File.Exists(_tempFilePath))
            {
                File.Delete(_tempFilePath);
            }
        }

        [Test]
        public void LoadSettings_WhenFileDoesNotExist_ShouldReturnDefaultSettings()
        {
            AppSettings settings = _service.LoadSettings();

            Assert.That(settings, Is.Not.Null);
            Assert.That(settings.Theme, Is.EqualTo("System"));
            Assert.That(settings.Language, Is.EqualTo("de"));
        }

        [Test]
        public void SaveSettings_ShouldCreateFileAndPersistValues()
        {
            AppSettings newSettings = new()
            {
                Theme = "Dark",
                Language = "en"
            };

            _service.SaveSettings(newSettings);

            Assert.That(File.Exists(_tempFilePath), Is.True);

            AppSettings loadedSettings = _service.LoadSettings();
            Assert.That(loadedSettings.Theme, Is.EqualTo("Dark"));
            Assert.That(loadedSettings.Language, Is.EqualTo("en"));
        }

        [Test]
        public void LoadSettings_WhenFileIsCorrupted_ShouldCatchExceptionAndReturnDefaults()
        {
            File.WriteAllText(_tempFilePath, "{ invalid_json: ... }");

            AppSettings settings = _service.LoadSettings();

            Assert.That(settings, Is.Not.Null);
            Assert.That(settings.Theme, Is.EqualTo("System"));
        }
    }
}
