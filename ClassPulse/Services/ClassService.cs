using At.luki0606.ClassPulse.Data;
using At.luki0606.ClassPulse.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace At.luki0606.ClassPulse.Services
{
    /// <summary>
    /// Default <see cref="IClassService"/> implementation backed by <see cref="AppDbContext"/>.
    /// </summary>
    public class ClassService : IClassService
    {
        private readonly AppDbContext _dbContext;

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="context">The database context.</param>
        public ClassService(AppDbContext context)
        {
            _dbContext = context;
        }

        /// <inheritdoc/>
        public async Task<Student> AddStudentToSchoolClassAsync(Guid schoolClassId, string firstName, string lastName, string? generalNotes = null)
        {
            Student student = new(firstName, lastName, schoolClassId, generalNotes);
            _dbContext.Students.Add(student);
            await _dbContext.SaveChangesAsync();
            return student;
        }

        /// <inheritdoc/>
        public async Task<SchoolClass> CreateClassAsync(string name, string schoolYear)
        {
            SchoolClass schoolClass = new(name, schoolYear);
            _dbContext.SchoolClasses.Add(schoolClass);
            await _dbContext.SaveChangesAsync();
            return schoolClass;
        }

        /// <inheritdoc/>
        public async Task<SchoolClass?> DeleteClassAsync(Guid classId)
        {
            SchoolClass? schoolClass = await _dbContext.SchoolClasses
                .FirstOrDefaultAsync(c => c.Id == classId);

            if (schoolClass == null)
            {
                return null;
            }

            _dbContext.SchoolClasses.Remove(schoolClass);
            await _dbContext.SaveChangesAsync();
            return schoolClass;
        }

        /// <inheritdoc/>
        public async Task<List<SchoolClass>> GetAllClassesAsync()
        {
            return await _dbContext.SchoolClasses
                .Include(c => c.Students)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<Student?> GetStudentDetailsAsync(Guid studentId)
        {
            return await _dbContext.Students
                .FirstOrDefaultAsync(s => s.Id == studentId);
        }

        /// <inheritdoc/>
        public async Task<List<Student>> SearchStudentAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return [];
            }

            string term = searchTerm.Trim();

            return await _dbContext.Students
                .Where(s => EF.Functions.Like(s.FirstName, $"%{term}%") ||
                            EF.Functions.Like(s.LastName, $"%{term}%"))
                .OrderBy(s => s.LastName)
                .ThenBy(s => s.FirstName)
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<List<Student>> GetStudentsByClassIdAsync(Guid classId)
        {
            return await _dbContext.Students
                .Where(s => s.SchoolClassId == classId)
                .OrderBy(s => s.LastName)
                .ThenBy(s => s.FirstName)
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<List<Subject>> GetAllSubjectsAsync()
        {
            return await _dbContext.Subjects
                .OrderBy(s => s.Name)
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<Subject> CreateSubjectAsync(string name, string code)
        {
            Subject subject = new(name, code);
            _dbContext.Subjects.Add(subject);
            await _dbContext.SaveChangesAsync();
            return subject;
        }

        /// <inheritdoc/>
        public async Task<Student?> RemoveStudentFromClassAsync(Guid schoolClassId, Guid studentId)
        {
            Student? student = await _dbContext.Students
                .FirstOrDefaultAsync(s => s.Id == studentId && s.SchoolClassId == schoolClassId);

            if (student == null)
            {
                return null;
            }

            _dbContext.Students.Remove(student);
            await _dbContext.SaveChangesAsync();
            return student;
        }

        /// <inheritdoc/>
        public async Task<Student?> UpdateStudentGeneralNotesAsync(Guid studentId, string? generalNotes)
        {
            Student? student = await _dbContext.Students.FirstOrDefaultAsync(s => s.Id == studentId);
            if (student == null)
            {
                return null;
            }

            student.UpdateGeneralNotes(generalNotes);
            await _dbContext.SaveChangesAsync();
            return student;
        }
    }
}