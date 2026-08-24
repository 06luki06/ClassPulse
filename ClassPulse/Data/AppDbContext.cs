using At.luki0606.ClassPulse.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace At.luki0606.ClassPulse.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<SchoolClass> SchoolClasses => Set<SchoolClass>();
        public DbSet<Student> Students => Set<Student>();
        public DbSet<Subject> Subjects => Set<Subject>();
        public DbSet<SubjectNote> SubjectNotes => Set<SubjectNote>();
        public DbSet<Assessment> Assessments => Set<Assessment>();

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

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
        }
    }
}
