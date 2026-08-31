using At.luki0606.ClassPulse.Data;
using At.luki0606.ClassPulse.Data.Entities;
using At.luki0606.ClassPulse.Services;
using Microsoft.EntityFrameworkCore;

namespace At.luki0606.ClassPulse.Tests.Services
{
    [TestFixture]
    public class ClassServiceTests
    {
        private AppDbContext _dbContext;
        private ClassService _classService;

        [SetUp]
        public void SetUp()
        {
            DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext = new AppDbContext(options);
            _classService = new ClassService(_dbContext);
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        [Test]
        public async Task CreateClassAsync_AddsClassToDatabaseAndReturnsIt()
        {
            string name = "4AHIF";
            string schoolYear = "2025/2026";

            SchoolClass result = await _classService.CreateClassAsync(name, schoolYear);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.Not.EqualTo(Guid.Empty));
            Assert.That(result.Name, Is.EqualTo(name));
            Assert.That(result.SchoolYear, Is.EqualTo(schoolYear));

            SchoolClass? dbClass = await _dbContext.SchoolClasses.FindAsync(result.Id);
            Assert.That(dbClass, Is.Not.Null);
        }

        [Test]
        public async Task AddStudentToSchoolClass_AddsStudentToDatabaseAndReturnsIt()
        {
            SchoolClass schoolClass = new("4AHIF", "2025/2026");
            _dbContext.SchoolClasses.Add(schoolClass);
            await _dbContext.SaveChangesAsync();

            string firstName = "Max";
            string lastName = "Mustermann";
            string notes = "Test note";

            Student result = await _classService.AddStudentToSchoolSclass(schoolClass.Id, firstName, lastName, notes);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.Not.EqualTo(Guid.Empty));
            Assert.That(result.FirstName, Is.EqualTo(firstName));
            Assert.That(result.LastName, Is.EqualTo(lastName));
            Assert.That(result.SchoolClassId, Is.EqualTo(schoolClass.Id));
            Assert.That(result.GeneralNotes, Is.EqualTo(notes));

            Student? dbStudent = await _dbContext.Students.FindAsync(result.Id);
            Assert.That(dbStudent, Is.Not.Null);
        }

        [Test]
        public async Task GetAllClassesAsync_ReturnsOrderedClassesWithStudents()
        {
            SchoolClass classA = new("B Klasse", "2025/2026");
            SchoolClass classB = new("A Klasse", "2025/2026");
            _dbContext.SchoolClasses.AddRange(classA, classB);
            await _dbContext.SaveChangesAsync();

            List<SchoolClass> result = await _classService.GetAllClassesAsync();

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result[0].Name, Is.EqualTo("A Klasse"));
            Assert.That(result[1].Name, Is.EqualTo("B Klasse"));
        }

        [Test]
        public async Task GetStudentDetailsAsync_WithValidId_ReturnsStudentWithIncludes()
        {
            SchoolClass schoolClass = new("4AHIF", "2025/2026");
            Student student = new("Max", "Mustermann", schoolClass.Id);
            _dbContext.SchoolClasses.Add(schoolClass);
            _dbContext.Students.Add(student);
            await _dbContext.SaveChangesAsync();

            Student? result = await _classService.GetStudentDetailsAsync(student.Id);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(student.Id));
            Assert.That(result.SchoolClass, Is.Not.Null);
            Assert.That(result.SchoolClass.Id, Is.EqualTo(schoolClass.Id));
            Assert.That(result.Assessments, Is.Empty);
            Assert.That(result.SubjectNotes, Is.Empty);
        }

        [Test]
        public async Task GetStudentDetailsAsync_WithInvalidId_ReturnsNull()
        {
            Student? result = await _classService.GetStudentDetailsAsync(Guid.NewGuid());

            Assert.That(result, Is.Null);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public async Task SearchStudentAsync_WithEmptySearchTerm_ReturnsEmptyList(string? invalidTerm)
        {
            List<Student> result = await _classService.SearchStudentAsync(invalidTerm);

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task SearchStudentAsync_WithMatchingTerm_ReturnsFilteredAndOrderedStudents()
        {
            SchoolClass school = new("4AHIF", "2025/2026");
            _dbContext.SchoolClasses.Add(school);
            await _dbContext.SaveChangesAsync();

            // In-memory db is case sensitive
            Student student1 = new("max", "huber", school.Id);
            Student student2 = new("maximilian", "auer", school.Id);
            Student student3 = new("anna", "schmidt", school.Id);
            _dbContext.Students.AddRange(student1, student2, student3);
            await _dbContext.SaveChangesAsync();

            List<Student> result = await _classService.SearchStudentAsync("max");

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result[0].LastName, Is.EqualTo("auer"));
            Assert.That(result[1].LastName, Is.EqualTo("huber"));
        }

        [Test]
        public async Task DeleteClassAsync_IdNotFound_ReturnsNull()
        {
            SchoolClass? school = await _classService.DeleteClassAsync(Guid.NewGuid());
            Assert.That(school, Is.Null);
        }

        [Test]
        public async Task DeleteClassAsync_ReturnsDeletedClass()
        {
            SchoolClass schoolClass = new("1a", "2026/2027");
            _dbContext.SchoolClasses.Add(schoolClass);
            await _dbContext.SaveChangesAsync();

            SchoolClass? deletedSchoolClass = await _classService.DeleteClassAsync(schoolClass.Id);
            Assert.That(deletedSchoolClass, Is.Not.Null);
            Assert.That(deletedSchoolClass.Id, Is.EqualTo(schoolClass.Id));
        }
    }
}