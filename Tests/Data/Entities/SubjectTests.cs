using At.luki0606.ClassPulse.Data.Entities;

namespace At.luki0606.ClassPulse.Tests.Data.Entities
{
    [TestFixture]
    public class SubjectTests
    {
        [Test]
        public void Constructor_WithValidArguments_SetsPropertiesCorrectlyTrimsAndUppersCode()
        {
            string name = "  Mathematik  ";
            string code = "  m  ";

            Subject subject = new(name, code);

            Assert.That(subject.Id, Is.Not.EqualTo(Guid.Empty));
            Assert.That(subject.Name, Is.EqualTo("Mathematik"));
            Assert.That(subject.Code, Is.EqualTo("M"));
            Assert.That(subject.Assessments, Is.Empty);
            Assert.That(subject.SubjectNotes, Is.Empty);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void Constructor_WithInvalidName_ThrowsArgumentNullException(string? invalidName)
        {
            void action()
            {
                new Subject(invalidName, "M");
            }

            Assert.Throws<ArgumentNullException>((Action)action);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void Constructor_WithInvalidCode_ThrowsArgumentNullException(string? invalidCode)
        {
            void action()
            {
                new Subject("Mathematik", invalidCode);
            }

            Assert.Throws<ArgumentNullException>((Action)action);
        }

        [Test]
        public void Update_WithValidArguments_UpdatesPropertiesTrimsAndUppersCode()
        {
            Subject subject = new("Mathematik", "M");

            string newName = "  Informatik  ";
            string newCode = "  inf  ";

            subject.Update(newName, newCode);

            Assert.That(subject.Name, Is.EqualTo("Informatik"));
            Assert.That(subject.Code, Is.EqualTo("INF"));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void Update_WithInvalidName_ThrowsArgumentNullException(string? invalidName)
        {
            Subject subject = new("Mathematik", "M");

            void action()
            {
                subject.Update(invalidName, "INF");
            }

            Assert.Throws<ArgumentNullException>((Action)action);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void Update_WithInvalidCode_ThrowsArgumentNullException(string? invalidCode)
        {
            Subject subject = new("Mathematik", "M");

            void action()
            {
                subject.Update("Informatik", invalidCode);
            }

            Assert.Throws<ArgumentNullException>((Action)action);
        }

        [Test]
        public void Collections_AreReadOnly()
        {
            Subject subject = new("Mathematik", "M");

            Assert.That(subject.Assessments, Is.InstanceOf<System.Collections.Generic.IReadOnlyCollection<Assessment>>());
            Assert.That(subject.SubjectNotes, Is.InstanceOf<System.Collections.Generic.IReadOnlyCollection<SubjectNote>>());
        }
    }
}