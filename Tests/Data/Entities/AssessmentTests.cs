using At.luki0606.ClassPulse.Data.Entities;

namespace At.luki0606.ClassPulse.Tests.Data.Entities
{
    [TestFixture]
    public class AssessmentTests
    {
        private readonly Guid _validStudentId = Guid.NewGuid();
        private readonly Guid _validSubjectId = Guid.NewGuid();

        [Test]
        public void Constructor_WithValidArguments_SetsPropertiesCorrectly()
        {
            string title = "Mathematik Schularbeit";
            DateTime date = new(2026, 6, 15, 0, 0, 0, DateTimeKind.Local);
            double grade = 2.0;
            double weight = 2.0;
            string positiveFeedback = "  Gut gemacht!  ";
            string improvementNotes = "  Mehr üben.  ";

            Assessment assessment = new(
                title,
                date,
                grade,
                _validStudentId,
                _validSubjectId,
                weight,
                positiveFeedback,
                improvementNotes);

            Assert.That(assessment.Id, Is.Not.EqualTo(Guid.Empty));
            Assert.That(assessment.Title, Is.EqualTo(title));
            Assert.That(assessment.Date, Is.EqualTo(date));
            Assert.That(assessment.Grade, Is.EqualTo(grade));
            Assert.That(assessment.Weight, Is.EqualTo(weight));
            Assert.That(assessment.PositiveFeedback, Is.EqualTo("Gut gemacht!"));
            Assert.That(assessment.ImprovementNotes, Is.EqualTo("Mehr üben."));
            Assert.That(assessment.StudentId, Is.EqualTo(_validStudentId));
            Assert.That(assessment.SubjectId, Is.EqualTo(_validSubjectId));
            Assert.That(assessment.Student, Is.Null);
            Assert.That(assessment.Subject, Is.Null);
        }

        [Test]
        public void Constructor_WithDefaultOptionalArguments_UsesDefaultValues()
        {
            Assessment assessment = new("Test", DateTime.Now, 1.5, _validStudentId, _validSubjectId);

            Assert.That(assessment.Weight, Is.EqualTo(1.0));
            Assert.That(assessment.PositiveFeedback, Is.Null);
            Assert.That(assessment.ImprovementNotes, Is.Null);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void Constructor_WithInvalidTitle_ThrowsArgumentNullException(string? invalidTitle)
        {
            void action()
            {
                new Assessment(invalidTitle, DateTime.Now, 2.0, _validStudentId, _validSubjectId);
            }

            Assert.Throws<ArgumentNullException>((Action)action);
        }

        [TestCase(0.9)]
        [TestCase(5.1)]
        [TestCase(-1.0)]
        public void Constructor_WithInvalidGrade_ThrowsArgumentOutOfRangeException(double invalidGrade)
        {
            void action()
            {
                new Assessment("Test", DateTime.Now, invalidGrade, _validStudentId, _validSubjectId);
            }

            Assert.Throws<ArgumentOutOfRangeException>((Action)action);
        }

        [TestCase(0.0)]
        [TestCase(-0.5)]
        [TestCase(-5.0)]
        public void Constructor_WithInvalidWeight_ThrowsArgumentOutOfRangeException(double invalidWeight)
        {
            void action()
            {
                new Assessment("Test", DateTime.Now, 2.0, _validStudentId, _validSubjectId, invalidWeight);
            }

            Assert.Throws<ArgumentOutOfRangeException>((Action)action);
        }

        [Test]
        public void Constructor_WithEmptyStudentId_ThrowsArgumentException()
        {
            void action()
            {
                new Assessment("Test", DateTime.Now, 2.0, Guid.Empty, _validSubjectId);
            }

            Assert.Throws<ArgumentException>((Action)action);
        }

        [Test]
        public void Constructor_WithEmptySubjectId_ThrowsArgumentException()
        {
            void action()
            {
                new Assessment("Test", DateTime.Now, 2.0, _validStudentId, Guid.Empty);
            }

            Assert.Throws<ArgumentException>((Action)action);
        }

        [Test]
        public void Update_WithValidArguments_UpdatesPropertiesAndTrimsStrings()
        {
            Assessment assessment = new("Old Title", DateTime.Now, 3.0, _validStudentId, _validSubjectId);

            string newTitle = "Updated Test";
            DateTime newDate = new(2026, 6, 20, 0, 0, 0, DateTimeKind.Local);
            double newGrade = 1.0;
            double newWeight = 1.5;
            string newPositive = "  Super!  ";
            string newImprovement = "  Weiter so!  ";

            assessment.Update(newTitle, newDate, newGrade, newWeight, newPositive, newImprovement);

            Assert.That(assessment.Title, Is.EqualTo(newTitle));
            Assert.That(assessment.Date, Is.EqualTo(newDate));
            Assert.That(assessment.Grade, Is.EqualTo(newGrade));
            Assert.That(assessment.Weight, Is.EqualTo(newWeight));
            Assert.That(assessment.PositiveFeedback, Is.EqualTo("Super!"));
            Assert.That(assessment.ImprovementNotes, Is.EqualTo("Weiter so!"));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void Update_WithInvalidTitle_ThrowsArgumentNullException(string? invalidTitle)
        {
            Assessment assessment = new("Valid Title", DateTime.Now, 2.0, _validStudentId, _validSubjectId);

            void action()
            {
                assessment.Update(invalidTitle, DateTime.Now, 2.0);
            }

            Assert.Throws<ArgumentNullException>((Action)action);
        }

        [TestCase(0.5)]
        [TestCase(5.5)]
        public void Update_WithInvalidGrade_ThrowsArgumentOutOfRangeException(double invalidGrade)
        {
            Assessment assessment = new("Valid Title", DateTime.Now, 2.0, _validStudentId, _validSubjectId);

            void action()
            {
                assessment.Update("New Title", DateTime.Now, invalidGrade);
            }

            Assert.Throws<ArgumentOutOfRangeException>((Action)action);
        }

        [TestCase(0.0)]
        [TestCase(-1.0)]
        public void Update_WithInvalidWeight_ThrowsArgumentOutOfRangeException(double invalidWeight)
        {
            Assessment assessment = new("Valid Title", DateTime.Now, 2.0, _validStudentId, _validSubjectId);

            void action()
            {
                assessment.Update("New Title", DateTime.Now, 2.0, invalidWeight);
            }

            Assert.Throws<ArgumentOutOfRangeException>((Action)action);
        }
    }
}