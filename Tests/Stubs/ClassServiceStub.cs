using At.luki0606.ClassPulse.Data.Entities;
using At.luki0606.ClassPulse.Services;

namespace At.luki0606.ClassPulse.Tests.Stubs
{
    internal class ClassServiceStub : IClassService
    {
        private readonly List<SchoolClass> _schoolClasses = [];
        private readonly List<Subject> _subjects = [];
        private readonly Dictionary<Guid, List<Student>> _classStudentsMap = [];

        public Task<SchoolClass> CreateClassAsync(string name, string schoolYear)
        {
            SchoolClass schoolClass = new(name, schoolYear);
            _schoolClasses.Add(schoolClass);
            _classStudentsMap[schoolClass.Id] = [];
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

        public Task<Student> AddStudentToSchoolClassAsync(Guid schoolClassId, string firstName, string lastName, string? generalNotes = null)
        {
            if (!_classStudentsMap.TryGetValue(schoolClassId, out List<Student>? value))
            {
                value = [];
                _classStudentsMap[schoolClassId] = value;
            }

            Student student = new(firstName, lastName, schoolClassId, generalNotes);
            value.Add(student);

            return Task.FromResult(student);
        }

        public Task<Student?> GetStudentDetailsAsync(Guid studentId)
        {
            Student? student = _classStudentsMap.Values
                .SelectMany(list => list)
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

        public Task<List<Student>> GetStudentsByClassIdAsync(Guid classId)
        {
            if (_classStudentsMap.TryGetValue(classId, out List<Student>? students))
            {
                List<Student> result = [.. students];
                return Task.FromResult(result);
            }

            return Task.FromResult(new List<Student>());
        }

        public Task<List<Subject>> GetAllSubjectsAsync()
        {
            return Task.FromResult(_subjects);
        }

        public Task<Subject> CreateSubjectAsync(string name, string code)
        {
            Subject subject = new(name, code);
            _subjects.Add(subject);
            return Task.FromResult(subject);
        }
    }
}