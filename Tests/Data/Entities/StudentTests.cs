using At.luki0606.ClassPulse.Data.Entities;

namespace At.luki0606.ClassPulse.Tests.Data.Entities
{
    [TestFixture]
    public class StudentTests
    {
        private readonly Guid _validSchoolClassId = Guid.NewGuid();

        [Test]
        public void Constructor_WithValidArguments_SetsPropertiesCorrectlyAndTrimsStrings()
        {
            string firstName = "  Max  ";
            string lastName = "  Mustermann  ";
            string generalNotes = "  Sehr aufmerksam.  ";

            Student student = new(firstName, lastName, _validSchoolClassId, generalNotes);

            Assert.That(student.Id, Is.Not.EqualTo(Guid.Empty));
            Assert.That(student.FirstName, Is.EqualTo("Max"));
            Assert.That(student.LastName, Is.EqualTo("Mustermann"));
            Assert.That(student.FullName, Is.EqualTo("Max Mustermann"));
            Assert.That(student.SchoolClassId, Is.EqualTo(_validSchoolClassId));
            Assert.That(student.GeneralNotes, Is.EqualTo("Sehr aufmerksam."));
            Assert.That(student.SchoolClass, Is.Null);
            Assert.That(student.SubjectNotes, Is.Empty);
            Assert.That(student.Assessments, Is.Empty);
        }

        [Test]
        public void Constructor_WithDefaultOptionalArguments_SetsGeneralNotesToNull()
        {
            Student student = new("Max", "Mustermann", _validSchoolClassId);

            Assert.That(student.GeneralNotes, Is.Null);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void Constructor_WithInvalidFirstName_ThrowsArgumentNullException(string? invalidFirstName)
        {
            void action()
            {
                new Student(invalidFirstName, "Mustermann", _validSchoolClassId);
            }

            Assert.Throws<ArgumentNullException>((Action)action);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void Constructor_WithInvalidLastName_ThrowsArgumentNullException(string? invalidLastName)
        {
            void action()
            {
                new Student("Max", invalidLastName, _validSchoolClassId);
            }

            Assert.Throws<ArgumentNullException>((Action)action);
        }

        [Test]
        public void Constructor_WithEmptySchoolClassId_ThrowsArgumentNullException()
        {
            static void action()
            {
                new Student("Max", "Mustermann", Guid.Empty);
            }

            Assert.Throws<ArgumentNullException>((Action)action);
        }

        [Test]
        public void UpdateDetails_WithValidArguments_UpdatesPropertiesAndTrimsStrings()
        {
            Student student = new("Max", "Mustermann", _validSchoolClassId, "Old notes");

            string newFirstName = "  Erika  ";
            string newLastName = "  Musterfrau  ";
            Guid newSchoolClassId = Guid.NewGuid();
            string newNotes = "  Neue Notiz.  ";

            student.UpdateDetails(newFirstName, newLastName, newSchoolClassId, newNotes);

            Assert.That(student.FirstName, Is.EqualTo("Erika"));
            Assert.That(student.LastName, Is.EqualTo("Musterfrau"));
            Assert.That(student.FullName, Is.EqualTo("Erika Musterfrau"));
            Assert.That(student.SchoolClassId, Is.EqualTo(newSchoolClassId));
            Assert.That(student.GeneralNotes, Is.EqualTo("Neue Notiz."));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void UpdateDetails_WithInvalidFirstName_ThrowsArgumentNullException(string? invalidFirstName)
        {
            Student student = new("Max", "Mustermann", _validSchoolClassId);

            void action()
            {
                student.UpdateDetails(invalidFirstName, "Mustermann", _validSchoolClassId);
            }

            Assert.Throws<ArgumentNullException>((Action)action);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void UpdateDetails_WithInvalidLastName_ThrowsArgumentNullException(string? invalidLastName)
        {
            Student student = new("Max", "Mustermann", _validSchoolClassId);

            void action()
            {
                student.UpdateDetails("Max", invalidLastName, _validSchoolClassId);
            }

            Assert.Throws<ArgumentNullException>((Action)action);
        }

        [Test]
        public void UpdateDetails_WithEmptySchoolClassId_ThrowsArgumentNullException()
        {
            Student student = new("Max", "Mustermann", _validSchoolClassId);

            void action()
            {
                student.UpdateDetails("Max", "Mustermann", Guid.Empty);
            }

            Assert.Throws<ArgumentNullException>((Action)action);
        }

        [TestCase("  Updated notes.  ", "Updated notes.")]
        [TestCase(null, null)]
        [TestCase("   ", "")]
        public void UpdateGeneralNotes_WithVariousInputs_UpdatesAndTrimsCorrectly(string? input, string? expected)
        {
            Student student = new("Max", "Mustermann", _validSchoolClassId, "Initial");

            student.UpdateGeneralNotes(input);

            Assert.That(student.GeneralNotes, Is.EqualTo(expected));
        }

        [Test]
        public void Collections_AreReadOnly()
        {
            Student student = new("Max", "Mustermann", _validSchoolClassId);

            Assert.That(student.SubjectNotes, Is.InstanceOf<System.Collections.Generic.IReadOnlyCollection<SubjectNote>>());
            Assert.That(student.Assessments, Is.InstanceOf<System.Collections.Generic.IReadOnlyCollection<Assessment>>());
        }
    }
}