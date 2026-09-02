using At.luki0606.ClassPulse.Data.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace At.luki0606.ClassPulse.Services
{
    public interface IClassService
    {
        Task<List<SchoolClass>> GetAllClassesAsync();
        Task<SchoolClass> CreateClassAsync(string name, string schoolYear);
        Task<SchoolClass?> DeleteClassAsync(Guid classId);
        Task<Student> AddStudentToSchoolClass(Guid schoolClassId, string firstName, string lastName, string? generalNotes = null);
        Task<List<Student>> SearchStudentAsync(string searchTerm);
        Task<Student?> GetStudentDetailsAsync(Guid studentId);
        Task<List<Student>> GetStudentsByClassIdAsync(Guid classId);
        Task<List<Subject>> GetAllSubjectsAsync();
        Task<Subject> CreateSubjectAsync(string name, string code);
    }
}
