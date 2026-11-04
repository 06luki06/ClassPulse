using At.luki0606.ClassPulse.Data;
using At.luki0606.ClassPulse.Data.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace At.luki0606.ClassPulse.Tests.Data
{
    [TestFixture]
    public class DatabaseSchemaUpdaterTests
    {
        private SqliteConnection _connection = null!;
        private AppDbContext _context = null!;

        [SetUp]
        public void SetUp()
        {
            _connection = new SqliteConnection("Data Source=:memory:");
            _connection.Open();
            DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(_connection)
                .Options;
            _context = new AppDbContext(options);
            _context.Database.EnsureCreated();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
            _connection.Dispose();
        }

        [Test]
        public void AddMissingColumns_ReAddsDroppedColumn_WithDefaultTrueForExistingRows()
        {
            Guid studentId = InsertStudent();
            _context.Database.ExecuteSqlRaw(
                "INSERT INTO Assessments (Id, Title, Date, Grade, Weight, HasParticipated, StudentId, SubjectId) " +
                "VALUES ({0}, 'Old', '2026-01-01', 2.0, 1.0, 1, {1}, {2})",
                Guid.NewGuid(), studentId, Guid.Parse("00000000-0000-0000-0000-000000000001"));
            _context.Database.ExecuteSqlRaw("ALTER TABLE Assessments DROP COLUMN HasParticipated;");
            Assert.That(GetColumns("Assessments"), Does.Not.Contain("HasParticipated"));

            DatabaseSchemaUpdater.AddMissingColumns(_context);

            Assert.That(GetColumns("Assessments"), Does.Contain("HasParticipated"));
            _context.ChangeTracker.Clear();
            Assessment loaded = _context.Assessments.Single();
            Assert.That(loaded.HasParticipated, Is.True);
        }

        [Test]
        public void AddMissingColumns_OnUpToDateSchema_DoesNothing()
        {
            List<string> before = GetColumns("Assessments");

            Assert.DoesNotThrow(() => DatabaseSchemaUpdater.AddMissingColumns(_context));

            Assert.That(GetColumns("Assessments"), Is.EquivalentTo(before));
        }

        [Test]
        public void HasParticipatedFalse_IsPersisted_DespiteDbDefaultTrue()
        {
            Guid studentId = InsertStudent();
            Assessment assessment = new("Missed", DateTime.Today, 0.0, studentId,
                Guid.Parse("00000000-0000-0000-0000-000000000001"));
            _context.Assessments.Add(assessment);
            _context.SaveChanges();
            _context.ChangeTracker.Clear();

            Assert.That(_context.Assessments.Single().HasParticipated, Is.False);
        }

        private Guid InsertStudent()
        {
            Guid classId = Guid.NewGuid();
            Guid studentId = Guid.NewGuid();
            _context.Database.ExecuteSqlRaw("INSERT INTO SchoolClasses (Id, Name, SchoolYear) VALUES ({0}, '1A', '2026')", classId);
            _context.Database.ExecuteSqlRaw(
                "INSERT INTO Students (Id, FirstName, LastName, SchoolClassId) VALUES ({0}, 'A', 'B', {1})",
                studentId, classId);
            return studentId;
        }

        private List<string> GetColumns(string table)
        {
            List<string> columns = [];
            using DbCommand command = _connection.CreateCommand();
            command.CommandText = $"PRAGMA table_info(\"{table}\");";
            using DbDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                columns.Add(reader.GetString(reader.GetOrdinal("name")));
            }
            return columns;
        }
    }
}
