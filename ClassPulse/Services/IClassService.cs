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
        Task<Student> AddStudentToSchoolSclass(Guid schoolClassId, string firstName, string lastName, string? generalNotes = null);
        Task<List<Student>> SearchStudentAsync(string searchTerm);
        Task<Student?> GetStudentDetailsAsync(Guid studentId);
    }
}
