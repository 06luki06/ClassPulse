using At.luki0606.ClassPulse.Data;
using At.luki0606.ClassPulse.Data.Entities;
using At.luki0606.ClassPulse.Services;
using Microsoft.EntityFrameworkCore;

namespace At.luki0606.ClassPulse.Tests.Services
{
    [TestFixture]
    public class AssessmentServiceTests
    {
        private AppDbContext _dbContext;
        private AssessmentService _assessmentService;

        [SetUp]
        public void SetUp()
        {
            DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext = new AppDbContext(options);
            _assessmentService = new AssessmentService(_dbContext);
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        [Test]
        public async Task AddSubjectNoteAsync_AddsNoteToDatabaseAndReturnsIt()
        {
            SchoolClass schoolClass = new("4AHIF", "2025/2026");
            Student student = new("Max", "Mustermann", schoolClass.Id);
            Subject subject = new("Mathematik", "M");

            _dbContext.SchoolClasses.Add(schoolClass);
            _dbContext.Students.Add(student);
            _dbContext.Subjects.Add(subject);
            await _dbContext.SaveChangesAsync();

            string text = " Needs more focus.  ";

            SubjectNote result = await _assessmentService.AddSubjectNoteAsync(student.Id, subject.Id, text);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.Not.EqualTo(Guid.Empty));
            Assert.That(result.Text, Is.EqualTo("Needs more focus."));
            Assert.That(result.StudentId, Is.EqualTo(student.Id));
            Assert.That(result.SubjectId, Is.EqualTo(subject.Id));

            SubjectNote? dbNote = await _dbContext.SubjectNotes.FindAsync(result.Id);
            Assert.That(dbNote, Is.Not.Null);
        }

        [Test]
        public void CalculateSubjectAverage_WithEmptyList_ReturnsZero()
        {
            double result = _assessmentService.CalculateSubjectAverage([]);

            Assert.That(result, Is.Zero);
        }

        [Test]
        public void CalculateSubjectAverage_WithAssessments_CalculatesCorrectWeightedAverage()
        {
            Guid studentId = Guid.NewGuid();
            Guid subjectId = Guid.NewGuid();
            DateTime date = DateTime.Now;

            List<Assessment> assessments =
            [
                new("Test 1", date, 2.0, studentId, subjectId, weight: 1.0), // Grade 2, Weight 1 -> Sum = 2
                new("Test 2", date, 4.0, studentId, subjectId, weight: 2.0)  // Grade 4, Weight 2 -> Sum = 8
            ];
            // Total weighted = 10, Total weight = 3 -> 10 / 3 = 3.333... -> 3.33

            double result = _assessmentService.CalculateSubjectAverage(assessments);

            Assert.That(result, Is.EqualTo(3.33));
        }

        [Test]
        public async Task CreateClassAssessmentAsync_WithStudents_CreatesAssessmentsForAllStudents()
        {
            SchoolClass schoolClass = new("4AHIF", "2025/2026");
            Subject subject = new("Informatik", "INF");
            _dbContext.SchoolClasses.Add(schoolClass);
            _dbContext.Subjects.Add(subject);

            Student student1 = new("Max", "Mustermann", schoolClass.Id);
            Student student2 = new("Erika", "Musterfrau", schoolClass.Id);
            _dbContext.Students.AddRange(student1, student2);
            await _dbContext.SaveChangesAsync();

            DateTime date = new(2026, 6, 1, 0, 0, 0, DateTimeKind.Local);
            List<Assessment> result = await _assessmentService.CreateClassAssessmentAsync(schoolClass.Id, subject.Id, "Schularbeit", date, weight: 2.0, defaultGrade: 2.5);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(await _dbContext.Assessments.CountAsync(), Is.EqualTo(2));

            foreach (Assessment assessment in result)
            {
                Assert.That(assessment.Title, Is.EqualTo("Schularbeit"));
                Assert.That(assessment.Grade, Is.EqualTo(2.5));
                Assert.That(assessment.Weight, Is.EqualTo(2.0));
                Assert.That(assessment.SubjectId, Is.EqualTo(subject.Id));
            }
        }

        [Test]
        public async Task CreateClassAssessmentAsync_WithNoStudents_ThrowsInvalidOperationException()
        {
            SchoolClass schoolClass = new("4AHIF", "2025/2026");
            Subject subject = new("Informatik", "INF");
            _dbContext.SchoolClasses.Add(schoolClass);
            _dbContext.Subjects.Add(subject);
            await _dbContext.SaveChangesAsync();

            async Task action()
            {
                await _assessmentService.CreateClassAssessmentAsync(schoolClass.Id, subject.Id, "Test", DateTime.Now);
            }

            InvalidOperationException ex = Assert.ThrowsAsync<InvalidOperationException>((Func<Task>)action);

            Assert.That(ex.Message, Is.EqualTo("No students found in the specified class."));
        }

        [TestCase(2, false)] // less than 3 assessments, cannot evaluate
        [TestCase(3, false)] // not a significant increase in grade (2.0 -> 2.3)
        [TestCase(4, true)]  // strong performance drop (1.0 -> 4.5)
        public void HasPerformanceDrop_EvaluatesCorrectly(int scenario, bool expectedResult)
        {
            Guid studentId = Guid.NewGuid();
            Guid subjectId = Guid.NewGuid();
            List<Assessment> assessments = [];

            if (scenario == 2)
            {
                assessments.Add(new("T1", new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Local), 1.0, studentId, subjectId));
                assessments.Add(new("T2", new DateTime(2026, 2, 1, 0, 0, 0, DateTimeKind.Local), 1.0, studentId, subjectId));
            }
            else if (scenario == 3)
            {
                assessments.Add(new("T1", new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Local), 2.0, studentId, subjectId));
                assessments.Add(new("T2", new DateTime(2026, 2, 1, 0, 0, 0, DateTimeKind.Local), 2.0, studentId, subjectId));
                assessments.Add(new("T3", new DateTime(2026, 3, 1, 0, 0, 0, DateTimeKind.Local), 2.3, studentId, subjectId));
            }
            else if (scenario == 4)
            {
                // older assessments have good grades, recent assessments have poor grades
                assessments.Add(new("T1", new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Local), 1.0, studentId, subjectId));
                assessments.Add(new("T2", new DateTime(2026, 2, 1, 0, 0, 0, DateTimeKind.Local), 1.0, studentId, subjectId));
                assessments.Add(new("T3", new DateTime(2026, 3, 1, 0, 0, 0, DateTimeKind.Local), 4.5, studentId, subjectId));
                assessments.Add(new("T4", new DateTime(2026, 4, 1, 0, 0, 0, DateTimeKind.Local), 4.5, studentId, subjectId));
            }

            bool result = _assessmentService.HasPerformanceDrop(assessments);

            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [Test]
        public async Task UpdateAssessmentGradeAsync_WithValidId_UpdatesGradeAndFeedback()
        {
            SchoolClass schoolClass = new("4AHIF", "2025/2026");
            Student student = new("Max", "Mustermann", schoolClass.Id);
            Subject subject = new("Mathematik", "M");
            Assessment assessment = new("Test", DateTime.Now, 3.0, student.Id, subject.Id);

            _dbContext.SchoolClasses.Add(schoolClass);
            _dbContext.Students.Add(student);
            _dbContext.Subjects.Add(subject);
            _dbContext.Assessments.Add(assessment);
            await _dbContext.SaveChangesAsync();

            await _assessmentService.UpdateAssessmentGradeAsync(assessment.Id, 1.5, "  Super  ", "  Keine  ");

            Assessment? updated = await _dbContext.Assessments.FindAsync(assessment.Id);
            Assert.That(updated, Is.Not.Null);
            Assert.That(updated.Grade, Is.EqualTo(1.5));
            Assert.That(updated.PositiveFeedback, Is.EqualTo("Super"));
            Assert.That(updated.ImprovementNotes, Is.EqualTo("Keine"));
        }

        [Test]
        public async Task UpdateAssessmentGradeAsync_WithInvalidId_ThrowsInvalidOperationException()
        {
            Subject subject = new("Mathematik", "M");
            _dbContext.Subjects.Add(subject);

            Student student = new("Max", "Mustermann", Guid.NewGuid());
            _dbContext.Students.Add(student);

            Assessment assessment = new("Test", DateTime.Now, 3.0, student.Id, subject.Id);
            _dbContext.Assessments.Add(assessment);

            await _dbContext.SaveChangesAsync();

            async Task action()
            {
                await _assessmentService.UpdateAssessmentGradeAsync(Guid.NewGuid(), 2.0);
            }

            InvalidOperationException? ex = Assert.ThrowsAsync<InvalidOperationException>((Func<Task>)action);

            Assert.That(ex.Message, Is.EqualTo("Assessment not found."));
        }
    }
}