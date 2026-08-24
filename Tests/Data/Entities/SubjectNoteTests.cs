using At.luki0606.ClassPulse.Data.Entities;

namespace At.luki0606.ClassPulse.Tests.Data.Entities
{
    [TestFixture]
    public class SubjectNoteTests
    {
        private readonly Guid _validStudentId = Guid.NewGuid();
        private readonly Guid _validSubjectId = Guid.NewGuid();

        [Test]
        public void Constructor_WithValidArguments_SetsPropertiesCorrectlyAndTrimsText()
        {
            string text = "  Needs improvement in algebra.  ";
            DateTime beforeCreation = DateTime.Now;

            SubjectNote note = new(text, _validStudentId, _validSubjectId);

            DateTime afterCreation = DateTime.Now;

            Assert.That(note.Id, Is.Not.EqualTo(Guid.Empty));
            Assert.That(note.Text, Is.EqualTo("Needs improvement in algebra."));
            Assert.That(note.CreatedAt, Is.EqualTo(beforeCreation).Within(TimeSpan.FromSeconds(1)));
            Assert.That(note.CreatedAt, Is.EqualTo(afterCreation).Within(TimeSpan.FromSeconds(1)));
            Assert.That(note.StudentId, Is.EqualTo(_validStudentId));
            Assert.That(note.SubjectId, Is.EqualTo(_validSubjectId));
            Assert.That(note.Student, Is.Null);
            Assert.That(note.Subject, Is.Null);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void Constructor_WithInvalidText_ThrowsArgumentNullException(string? invalidText)
        {
            void action()
            {
                new SubjectNote(invalidText, _validStudentId, _validSubjectId);
            }

            Assert.Throws<ArgumentNullException>((Action)action);
        }

        [Test]
        public void Constructor_WithEmptyStudentId_ThrowsArgumentException()
        {
            void action()
            {
                new SubjectNote("Valid text", Guid.Empty, _validSubjectId);
            }

            Assert.Throws<ArgumentException>((Action)action);
        }

        [Test]
        public void Constructor_WithEmptySubjectId_ThrowsArgumentException()
        {
            void action()
            {
                new SubjectNote("Valid text", _validStudentId, Guid.Empty);
            }

            Assert.Throws<ArgumentException>((Action)action);
        }

        [Test]
        public void UpdateText_WithValidText_UpdatesTextAndTrims()
        {
            SubjectNote note = new("Old text", _validStudentId, _validSubjectId);
            string newText = "  Updated note text.  ";

            note.UpdateText(newText);

            Assert.That(note.Text, Is.EqualTo("Updated note text."));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void UpdateText_WithInvalidText_ThrowsArgumentNullException(string? invalidText)
        {
            SubjectNote note = new("Initial text", _validStudentId, _validSubjectId);

            void action()
            {
                note.UpdateText(invalidText);
            }

            Assert.Throws<ArgumentNullException>((Action)action);
        }
    }
}