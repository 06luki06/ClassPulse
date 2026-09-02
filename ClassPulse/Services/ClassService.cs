using At.luki0606.ClassPulse.Data;
using At.luki0606.ClassPulse.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace At.luki0606.ClassPulse.Services
{
    public class ClassService : IClassService
    {
        private readonly AppDbContext _dbContext;

        public ClassService(AppDbContext context)
        {
            _dbContext = context;
        }

        public async Task<Student> AddStudentToSchoolSclass(Guid schoolClassId, string firstName, string lastName, string? generalNotes = null)
        {
            Student student = new(firstName, lastName, schoolClassId, generalNotes);
            _dbContext.Students.Add(student);
            await _dbContext.SaveChangesAsync();
            return student;
        }

        public async Task<SchoolClass> CreateClassAsync(string name, string schoolYear)
        {
            SchoolClass schoolClass = new(name, schoolYear);
            _dbContext.SchoolClasses.Add(schoolClass);
            await _dbContext.SaveChangesAsync();
            return schoolClass;
        }

        public async Task<SchoolClass?> DeleteClassAsync(Guid classId)
        {
            SchoolClass? schoolClass = await _dbContext.SchoolClasses
                .Include(c => c.Students)
                .FirstOrDefaultAsync(c => c.Id == classId);

            if (schoolClass == null)
            {
                return null;
            }

            _dbContext.SchoolClasses.Remove(schoolClass);
            await _dbContext.SaveChangesAsync();
            return schoolClass;
        }

        public async Task<List<SchoolClass>> GetAllClassesAsync()
        {
            return await _dbContext.SchoolClasses
                .Include(c => c.Students)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<Student?> GetStudentDetailsAsync(Guid studentId)
        {
            return await _dbContext.Students
                .Include(s => s.SchoolClass)
                .Include(s => s.Assessments)
                    .ThenInclude(a => a.Subject)
                .Include(s => s.SubjectNotes)
                    .ThenInclude(sn => sn.Subject)
                .FirstOrDefaultAsync(s => s.Id == studentId);
        }

        public async Task<List<Student>> SearchStudentAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return [];
            }

            string term = searchTerm.Trim().ToLower();

            return await _dbContext.Students
                .Include(s => s.SchoolClass)
                .Where(s => (s.FirstName != null && s.FirstName.Contains(term, StringComparison.CurrentCultureIgnoreCase)) ||
                            (s.LastName != null && s.LastName.Contains(term, StringComparison.CurrentCultureIgnoreCase)))
                .OrderBy(s => s.LastName)
                .ThenBy(s => s.FirstName)
                .ToListAsync();
        }

        public async Task<List<Student>> GetStudentsByClassIdAsync(Guid classId)
        {
            return await _dbContext.Students
                .Include(s => s.SchoolClass)
                .Where(s => s.SchoolClassId == classId)
                .OrderBy(s => s.LastName)
                .ThenBy(s => s.FirstName)
                .ToListAsync();
        }

        public async Task<List<Subject>> GetAllSubjectsAsync()
        {
            return await _dbContext.Subjects
                .OrderBy(s => s.Name)
                .ToListAsync();
        }
    }
}