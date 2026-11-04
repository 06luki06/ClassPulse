using At.luki0606.ClassPulse.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace At.luki0606.ClassPulse.Data
{
    /// <summary>
    /// Entity Framework Core session for the application's SQLite database.
    /// </summary>
    public class AppDbContext : DbContext
    {
        /// <summary>
        /// Gets the school classes.
        /// </summary>
        public DbSet<SchoolClass> SchoolClasses => Set<SchoolClass>();
        /// <summary>
        /// Gets the students.
        /// </summary>
        public DbSet<Student> Students => Set<Student>();
        /// <summary>
        /// Gets the subjects.
        /// </summary>
        public DbSet<Subject> Subjects => Set<Subject>();
        /// <summary>
        /// Gets the subject notes.
        /// </summary>
        public DbSet<SubjectNote> SubjectNotes => Set<SubjectNote>();
        /// <summary>
        /// Gets the assessments.
        /// </summary>
        public DbSet<Assessment> Assessments => Set<Assessment>();

        /// <summary>
        /// Initializes a new instance with the given options.
        /// </summary>
        /// <param name="options">The context options, including the database provider.</param>
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        /// <summary>
        /// Configures seed subjects, relationships, delete behavior and automatically included navigations.
        /// </summary>
        /// <param name="modelBuilder">The builder for the entity model.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            Guid mathId = Guid.Parse("00000000-0000-0000-0000-000000000001");
            Guid germanId = Guid.Parse("00000000-0000-0000-0000-000000000002");
            Guid scienceId = Guid.Parse("00000000-0000-0000-0000-000000000003");

            modelBuilder.Entity<Subject>().HasData(
                new { Id = mathId, Name = "Mathematik", Code = "M" },
                new { Id = germanId, Name = "Deutsch", Code = "D" },
                new { Id = scienceId, Name = "Sachunterricht", Code = "SU" }
            );

            modelBuilder.Entity<Student>()
                .HasOne(s => s.SchoolClass)
                .WithMany(c => c.Students)
                .HasForeignKey(s => s.SchoolClassId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Assessment>()
                .HasOne(a => a.Student)
                .WithMany(s => s.Assessments)
                .HasForeignKey(a => a.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Assessment>()
                .HasOne(a => a.Subject)
                .WithMany(s => s.Assessments)
                .HasForeignKey(a => a.SubjectId)
                .OnDelete(DeleteBehavior.Restrict);

            // Existing assessments were all "participated". ValueGeneratedNever ensures EF
            // always writes the actual value (otherwise 'false' would be replaced by the DB default).
            modelBuilder.Entity<Assessment>()
                .Property(a => a.HasParticipated)
                .HasDefaultValue(true)
                .ValueGeneratedNever();

            modelBuilder.Entity<SubjectNote>()
                .HasOne(sn => sn.Student)
                .WithMany(s => s.SubjectNotes)
                .HasForeignKey(sn => sn.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SubjectNote>()
                .HasOne(sn => sn.Subject)
                .WithMany(s => s.SubjectNotes)
                .HasForeignKey(sn => sn.SubjectId)
                .OnDelete(DeleteBehavior.Restrict);

            // ==========================================
            // Auto-includes for automatic loading of related entities
            // ==========================================

            modelBuilder.Entity<Student>(entity =>
            {
                entity.Navigation(s => s.SchoolClass).AutoInclude();
                entity.Navigation(s => s.Assessments).AutoInclude();
                entity.Navigation(s => s.SubjectNotes).AutoInclude();
            });

            modelBuilder.Entity<Assessment>(entity =>
            {
                entity.Navigation(a => a.Subject).AutoInclude();
            });

            modelBuilder.Entity<SubjectNote>(entity =>
            {
                entity.Navigation(sn => sn.Subject).AutoInclude();
            });
        }
    }
}
