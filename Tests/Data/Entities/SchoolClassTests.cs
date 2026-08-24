using At.luki0606.ClassPulse.Data.Entities;

namespace At.luki0606.ClassPulse.Tests.Data.Entities
{
    [TestFixture]
    public class SchoolClassTests
    {
        [Test]
        public void Constructor_WithValidArguments_SetsPropertiesCorrectlyAndTrimsName()
        {
            string name = "  4AHIF  ";
            string schoolYear = "2025/2026";

            SchoolClass schoolClass = new(name, schoolYear);

            Assert.Multiple(() =>
            {
                Assert.That(schoolClass.Id, Is.Not.EqualTo(Guid.Empty));
                Assert.That(schoolClass.Name, Is.EqualTo("4AHIF"));
                Assert.That(schoolClass.SchoolYear, Is.EqualTo(schoolYear));
                Assert.That(schoolClass.Students, Is.Not.Null);
            });
            Assert.That(schoolClass.Students, Is.Empty);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void Constructor_WithInvalidName_ThrowsArgumentNullException(string? invalidName)
        {
            Assert.Throws<ArgumentNullException>(() =>
                new SchoolClass(invalidName, "2025/2026"));
        }

        [TestCase("2025")]
        [TestCase("25/26")]
        [TestCase("2025-2026")]
        [TestCase("2025/26")]
        [TestCase("ABCD/EFGH")]
        [TestCase("2025/2026/2027")]
        [TestCase("")]
        [TestCase(null)]
        public void Constructor_WithInvalidSchoolYearFormat_ThrowsArgumentException(string? invalidSchoolYear)
        {
            Assert.Throws<ArgumentException>(() =>
                new SchoolClass("4AHIF", invalidSchoolYear));
        }

        [Test]
        public void Update_WithValidArguments_UpdatesPropertiesAndTrimsName()
        {
            SchoolClass schoolClass = new("3AHIF", "2024/2025");
            string newName = "  4AHIF  ";
            string newSchoolYear = "2025/2026";

            schoolClass.Update(newName, newSchoolYear);

            Assert.Multiple(() =>
            {
                Assert.That(schoolClass.Name, Is.EqualTo("4AHIF"));
                Assert.That(schoolClass.SchoolYear, Is.EqualTo(newSchoolYear));
            });
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void Update_WithInvalidName_ThrowsArgumentNullException(string? invalidName)
        {
            SchoolClass schoolClass = new("4AHIF", "2025/2026");

            Assert.Throws<ArgumentNullException>(() =>
                schoolClass.Update(invalidName, "2025/2026"));
        }

        [TestCase("2026")]
        [TestCase("25-26")]
        [TestCase("2025/26")]
        public void Update_WithInvalidSchoolYearFormat_ThrowsArgumentException(string invalidSchoolYear)
        {
            SchoolClass schoolClass = new("4AHIF", "2025/2026");

            Assert.Throws<ArgumentException>(() =>
                schoolClass.Update("4AHIF", invalidSchoolYear));
        }

        [Test]
        public void Students_Collection_IsReadOnly()
        {
            SchoolClass schoolClass = new("4AHIF", "2025/2026");

            Assert.That(schoolClass.Students, Is.Not.Null);
            Assert.That(schoolClass.Students, Is.InstanceOf<IReadOnlyCollection<Student>>());
        }
    }
}