namespace At.luki0606.ClassPulse.Tests
{
    [TestFixture]
    internal class UtilsTests
    {
        [Test]
        public void GetAppdataFolderPath_ReturnsExpectedPath()
        {
            string result = Utils.GetAppdataFolderPath();

            string expectedAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string expectedPath = Path.Combine(expectedAppData, "ClassPulse");

            Assert.That(result, Is.EqualTo(expectedPath));
        }

        [Test]
        public void GetAppdataFolderPath_CreatesDirectoryOnDisk()
        {
            string result = Utils.GetAppdataFolderPath();
            Assert.That(Directory.Exists(result), Is.True);
        }
    }
}
