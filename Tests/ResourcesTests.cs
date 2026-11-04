using System.Collections;
using System.Globalization;
using System.Resources;
using AppResources = At.luki0606.ClassPulse.Resources.Resources;

namespace At.luki0606.ClassPulse.Tests
{
    [TestFixture]
    public class ResourcesTests
    {
        [Test]
        public void GermanAndNeutralResources_ContainTheSameKeys()
        {
            Dictionary<string, string> neutral = ReadResources(CultureInfo.InvariantCulture);
            Dictionary<string, string> german = ReadResources(new CultureInfo("de"));

            Assert.That(german.Keys, Is.EquivalentTo(neutral.Keys));
        }

        [TestCase("")]
        [TestCase("de")]
        public void Resources_HaveNoBlankValues(string cultureName)
        {
            Dictionary<string, string> resources = ReadResources(new CultureInfo(cultureName));

            List<string> blankKeys = [.. resources.Where(r => string.IsNullOrWhiteSpace(r.Value)).Select(r => r.Key)];

            Assert.That(blankKeys, Is.Empty);
        }

        private static Dictionary<string, string> ReadResources(CultureInfo culture)
        {
            ResourceSet resourceSet = AppResources.ResourceManager.GetResourceSet(culture, createIfNotExists: true, tryParents: false)
                ?? throw new InvalidOperationException($"No resources found for culture '{culture.Name}'.");

            Dictionary<string, string> resources = [];
            foreach (DictionaryEntry entry in resourceSet)
            {
                resources[(string)entry.Key] = entry.Value as string ?? string.Empty;
            }

            return resources;
        }
    }
}
