using At.luki0606.ClassPulse.Data;
using At.luki0606.ClassPulse.Data.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace At.luki0606.ClassPulse.Tests.Data
{
    /// <summary>
    /// Opens a database file in the format of the oldest installed copies: created with EF Core 8.0.30
    /// by commit e195f99^, before <see cref="Assessment.HasParticipated"/> existed. The schema and the
    /// value formats below are copied from such a database. Do not regenerate them with a newer EF Core.
    /// </summary>
    [TestFixture]
    public class DatabaseSchemaUpdaterOldFormatTests
    {
        private const string _oldFormatSchema = """
            CREATE TABLE "SchoolClasses" (
                "Id" TEXT NOT NULL CONSTRAINT "PK_SchoolClasses" PRIMARY KEY,
                "Name" TEXT NOT NULL,
                "SchoolYear" TEXT NOT NULL
            );
            CREATE TABLE "Subjects" (
                "Id" TEXT NOT NULL CONSTRAINT "PK_Subjects" PRIMARY KEY,
                "Name" TEXT NOT NULL,
                "Code" TEXT NOT NULL
            );
            CREATE TABLE "Students" (
                "Id" TEXT NOT NULL CONSTRAINT "PK_Students" PRIMARY KEY,
                "FirstName" TEXT NULL,
                "LastName" TEXT NULL,
                "GeneralNotes" TEXT NULL,
                "SchoolClassId" TEXT NOT NULL,
                CONSTRAINT "FK_Students_SchoolClasses_SchoolClassId" FOREIGN KEY ("SchoolClassId") REFERENCES "SchoolClasses" ("Id") ON DELETE CASCADE
            );
            CREATE TABLE "Assessments" (
                "Id" TEXT NOT NULL CONSTRAINT "PK_Assessments" PRIMARY KEY,
                "Title" TEXT NOT NULL,
                "Date" TEXT NOT NULL,
                "Grade" REAL NOT NULL,
                "Weight" REAL NOT NULL,
                "PositiveFeedback" TEXT NULL,
                "ImprovementNotes" TEXT NULL,
                "StudentId" TEXT NOT NULL,
                "SubjectId" TEXT NOT NULL,
                CONSTRAINT "FK_Assessments_Students_StudentId" FOREIGN KEY ("StudentId") REFERENCES "Students" ("Id") ON DELETE CASCADE,
                CONSTRAINT "FK_Assessments_Subjects_SubjectId" FOREIGN KEY ("SubjectId") REFERENCES "Subjects" ("Id") ON DELETE RESTRICT
            );
            CREATE TABLE "SubjectNotes" (
                "Id" TEXT NOT NULL CONSTRAINT "PK_SubjectNotes" PRIMARY KEY,
                "Text" TEXT NULL,
                "CreatedAt" TEXT NOT NULL,
                "StudentId" TEXT NOT NULL,
                "SubjectId" TEXT NOT NULL,
                CONSTRAINT "FK_SubjectNotes_Students_StudentId" FOREIGN KEY ("StudentId") REFERENCES "Students" ("Id") ON DELETE CASCADE,
                CONSTRAINT "FK_SubjectNotes_Subjects_SubjectId" FOREIGN KEY ("SubjectId") REFERENCES "Subjects" ("Id") ON DELETE RESTRICT
            );
            CREATE INDEX "IX_Assessments_StudentId" ON "Assessments" ("StudentId");
            CREATE INDEX "IX_Assessments_SubjectId" ON "Assessments" ("SubjectId");
            CREATE INDEX "IX_Students_SchoolClassId" ON "Students" ("SchoolClassId");
            CREATE INDEX "IX_SubjectNotes_StudentId" ON "SubjectNotes" ("StudentId");
            CREATE INDEX "IX_SubjectNotes_SubjectId" ON "SubjectNotes" ("SubjectId");
            """;

        private const string _oldFormatData = """
            INSERT INTO "SchoolClasses" VALUES ('E5C94588-0215-472F-807A-BF808479594E', '1A', '2025/2026');
            INSERT INTO "Subjects" VALUES ('00000000-0000-0000-0000-000000000001', 'Mathematik', 'M');
            INSERT INTO "Subjects" VALUES ('00000000-0000-0000-0000-000000000002', 'Deutsch', 'D');
            INSERT INTO "Subjects" VALUES ('00000000-0000-0000-0000-000000000003', 'Sachunterricht', 'SU');
            INSERT INTO "Students" VALUES ('0D6FB341-D2A5-4CB7-A3D5-61B1C09DE3A1', 'Max', 'Mustermann', 'Testnotiz', 'E5C94588-0215-472F-807A-BF808479594E');
            INSERT INTO "Assessments" VALUES ('38EAEFD1-FB20-42C8-8FE7-2FEC357AFA75', 'Ansage', '2026-03-15 08:30:00', 1.0, 1.0, NULL, NULL, '0D6FB341-D2A5-4CB7-A3D5-61B1C09DE3A1', '00000000-0000-0000-0000-000000000001');
            INSERT INTO "Assessments" VALUES ('3C6F5C84-536E-4AD7-B01D-B2CF607DFDE1', 'Schularbeit', '2026-03-14 00:00:00', 2.5, 1.5, NULL, NULL, '0D6FB341-D2A5-4CB7-A3D5-61B1C09DE3A1', '00000000-0000-0000-0000-000000000001');
            INSERT INTO "SubjectNotes" VALUES ('43CE5D0E-56A4-46B1-9123-991F145C11BD', 'Liest gerne', '2026-11-05 19:37:07.7050773', '0D6FB341-D2A5-4CB7-A3D5-61B1C09DE3A1', '00000000-0000-0000-0000-000000000001');
            """;

        private static readonly string[] _tables = ["SchoolClasses", "Subjects", "Students", "Assessments", "SubjectNotes"];
        private static readonly Guid _studentId = Guid.Parse("0D6FB341-D2A5-4CB7-A3D5-61B1C09DE3A1");
        private static readonly Guid _mathId = Guid.Parse("00000000-0000-0000-0000-000000000001");

        private string _directory = null!;

        [SetUp]
        public void SetUp()
        {
            _directory = Directory.CreateTempSubdirectory("ClassPulseTests").FullName;
            ExecuteOnFile(Path.Combine(_directory, "old.db"), _oldFormatSchema + _oldFormatData);
        }

        [TearDown]
        public void TearDown()
        {
            Directory.Delete(_directory, recursive: true);
        }

        [Test]
        public void EnsureCreatedAndUpToDate_OnOldFormatFile_AddsMissingColumnWithDefaultTrue()
        {
            UpdateOldDatabase();

            using AppDbContext context = CreateContext("old.db");
            List<Assessment> assessments = [.. context.Assessments];

            Assert.Multiple(() =>
            {
                Assert.That(GetColumns("old.db", "Assessments"), Does.Contain("HasParticipated|INTEGER|1|1"));
                Assert.That(assessments, Has.Count.EqualTo(2));
                Assert.That(assessments.Select(a => a.HasParticipated), Is.All.True);
            });
        }

        [Test]
        public void EnsureCreatedAndUpToDate_OnOldFormatFile_KeepsExistingValues()
        {
            UpdateOldDatabase();

            using AppDbContext context = CreateContext("old.db");
            Student student = context.Students.Single(s => s.Id == _studentId);
            Assessment exam = student.Assessments.Single(a => a.Title == "Schularbeit");
            SubjectNote note = student.SubjectNotes.Single();

            Assert.Multiple(() =>
            {
                Assert.That(student.LastName, Is.EqualTo("Mustermann"));
                Assert.That(student.GeneralNotes, Is.EqualTo("Testnotiz"));
                Assert.That(student.SchoolClass?.SchoolYear, Is.EqualTo("2025/2026"));
                Assert.That(exam.Grade, Is.EqualTo(2.5));
                Assert.That(exam.Weight, Is.EqualTo(1.5));
                Assert.That(exam.Subject?.Code, Is.EqualTo("M"));
                Assert.That(context.Subjects.Count(), Is.EqualTo(3));
                Assert.That(note.Text, Is.EqualTo("Liest gerne"));
            });
        }

        [Test]
        public void EnsureCreatedAndUpToDate_OnOldFormatFile_KeepsDatesWithoutTimeZoneShift()
        {
            UpdateOldDatabase();

            using AppDbContext context = CreateContext("old.db");
            Assessment dictation = context.Assessments.Single(a => a.Title == "Ansage");
            SubjectNote note = context.SubjectNotes.Single();

            Assert.Multiple(() =>
            {
                Assert.That(dictation.Date, Is.EqualTo(new DateTime(2026, 3, 15, 8, 30, 0)));
                Assert.That(dictation.Date.Kind, Is.EqualTo(DateTimeKind.Unspecified));
                Assert.That(note.CreatedAt, Is.EqualTo(new DateTime(2026, 11, 5, 19, 37, 7).AddTicks(7050773)));
            });
        }

        [Test]
        public void EnsureCreatedAndUpToDate_OnOldFormatFile_AcceptsNewDataAfterReopening()
        {
            UpdateOldDatabase();

            using (AppDbContext context = CreateContext("old.db"))
            {
                context.Assessments.Add(new Assessment("Missed", new DateTime(2026, 11, 5), 0.0, _studentId, _mathId));
                context.SaveChanges();
            }

            UpdateOldDatabase();

            using AppDbContext reopened = CreateContext("old.db");
            Assert.Multiple(() =>
            {
                Assert.That(reopened.Assessments.Count(), Is.EqualTo(3));
                Assert.That(reopened.Assessments.Single(a => a.Title == "Missed").HasParticipated, Is.False);
            });
        }

        [Test]
        public void EnsureCreatedAndUpToDate_OnOldFormatFile_MatchesSchemaOfNewDatabase()
        {
            UpdateOldDatabase();
            using (AppDbContext context = CreateContext("new.db"))
            {
                DatabaseSchemaUpdater.EnsureCreatedAndUpToDate(context);
            }

            Assert.Multiple(() =>
            {
                foreach (string table in _tables)
                {
                    Assert.That(GetColumns("old.db", table), Is.EquivalentTo(GetColumns("new.db", table)), table);
                }
            });
        }

        private void UpdateOldDatabase()
        {
            using AppDbContext context = CreateContext("old.db");
            DatabaseSchemaUpdater.EnsureCreatedAndUpToDate(context);
        }

        private AppDbContext CreateContext(string fileName)
        {
            DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(GetConnectionString(Path.Combine(_directory, fileName)))
                .Options;
            return new AppDbContext(options);
        }

        private static void ExecuteOnFile(string path, string sql)
        {
            using SqliteConnection connection = new(GetConnectionString(path));
            connection.Open();
            using DbCommand command = connection.CreateCommand();
            command.CommandText = sql;
            _ = command.ExecuteNonQuery();
        }

        private List<string> GetColumns(string fileName, string table)
        {
            using SqliteConnection connection = new(GetConnectionString(Path.Combine(_directory, fileName)));
            connection.Open();
            using DbCommand command = connection.CreateCommand();
            command.CommandText = $"PRAGMA table_info(\"{table}\");";
            using DbDataReader reader = command.ExecuteReader();

            List<string> columns = [];
            while (reader.Read())
            {
                int defaultOrdinal = reader.GetOrdinal("dflt_value");
                string defaultValue = reader.IsDBNull(defaultOrdinal) ? "NULL" : reader.GetString(defaultOrdinal);
                columns.Add($"{reader.GetString(reader.GetOrdinal("name"))}|{reader.GetString(reader.GetOrdinal("type"))}|{reader.GetInt32(reader.GetOrdinal("notnull"))}|{defaultValue}");
            }

            return columns;
        }

        // Pooling is off so that no connection keeps the file locked when TearDown deletes it.
        private static string GetConnectionString(string path)
        {
            return new SqliteConnectionStringBuilder { DataSource = path, Pooling = false }.ToString();
        }
    }
}
