using At.luki0606.ClassPulse.Data.Entities;
using At.luki0606.ClassPulse.Services;

namespace At.luki0606.ClassPulse.Tests.Stubs
{
    internal class ClassServiceStub : IClassService
    {
        private readonly List<SchoolClass> _schoolClasses = [];

        public Task<SchoolClass> CreateClassAsync(string name, string schoolYear)
        {
            SchoolClass schoolClass = new(name, schoolYear);

            _schoolClasses.Add(schoolClass);
            return Task.FromResult(schoolClass);
        }

        public Task<List<SchoolClass>> GetAllClassesAsync()
        {
            return Task.FromResult(_schoolClasses.ToList());
        }

        public Task<SchoolClass?> DeleteClassAsync(Guid classId)
        {
            SchoolClass? schoolClass = _schoolClasses.FirstOrDefault(c => c.Id == classId);
            if (schoolClass != null)
            {
                _schoolClasses.Remove(schoolClass);
            }

            return Task.FromResult(schoolClass);
        }

        public Task<Student> AddStudentToSchoolSclass(Guid schoolClassId, string firstName, string lastName, string? generalNotes = null)
        {
            SchoolClass schoolClass = _schoolClasses.FirstOrDefault(c => c.Id == schoolClassId)
                ?? throw new KeyNotFoundException($"SchoolClass with ID {schoolClassId} not found.");

            Student student = new(firstName, lastName, schoolClassId, generalNotes);

            IEnumerable<Student> _ = schoolClass.Students.Append(student);

            return Task.FromResult(student);
        }

        public Task<Student?> GetStudentDetailsAsync(Guid studentId)
        {
            Student? student = _schoolClasses
                .SelectMany(c => c.Students ?? Enumerable.Empty<Student>())
                .FirstOrDefault(s => s.Id == studentId);

            return Task.FromResult(student);
        }

        public Task<List<Student>> SearchStudentAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return Task.FromResult(new List<Student>());
            }

            List<Student> results = [.. _schoolClasses
                .SelectMany(c => c.Students ?? Enumerable.Empty<Student>())
                .Where(s => s.FirstName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                            s.LastName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))];

            return Task.FromResult(results);
        }
    }
}