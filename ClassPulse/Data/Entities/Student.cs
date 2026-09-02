using System;
using System.Collections.Generic;
using System.Linq;

namespace At.luki0606.ClassPulse.Data.Entities
{
    public class Student
    {
        public Guid Id { get; private set; }
        public string? FirstName { get; private set; }
        public string? LastName { get; private set; }
        public string? GeneralNotes { get; private set; }

        public Guid SchoolClassId { get; private set; }
        public SchoolClass? SchoolClass { get; private set; } = null;

        private readonly List<SubjectNote> _subjectNotes = [];
        public IReadOnlyCollection<SubjectNote> SubjectNotes => _subjectNotes.AsReadOnly();

        private readonly List<Assessment> _assessments = [];
        public IReadOnlyCollection<Assessment> Assessments => _assessments.AsReadOnly();

        public string FullName => $"{FirstName} {LastName}";

        private Student()
        {
            FirstName = string.Empty;
            LastName = string.Empty;
        }

        public Student(string firstName, string lastName, Guid schoolClassId, string? generalNotes = null)
        {
            ValidateName(firstName);
            ValidateName(lastName);
            ValidateSchoolClassId(schoolClassId);
            Id = Guid.NewGuid();
            FirstName = firstName.Trim();
            LastName = lastName.Trim();
            SchoolClassId = schoolClassId;
            GeneralNotes = generalNotes?.Trim();
        }

        public void UpdateDetails(string firstName, string lastName, Guid schoolClassId, string? generalNotes = null)
        {
            ValidateName(firstName);
            ValidateName(lastName);
            ValidateSchoolClassId(schoolClassId);
            FirstName = firstName.Trim();
            LastName = lastName.Trim();
            SchoolClassId = schoolClassId;
            GeneralNotes = generalNotes?.Trim();
        }

        public void UpdateGeneralNotes(string? generalNotes)
        {
            GeneralNotes = generalNotes?.Trim();
        }


        private static void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }
        }

        private static void ValidateSchoolClassId(Guid schoolClassId)
        {
            if (schoolClassId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(schoolClassId));
            }
        }

        public IEnumerable<Assessment> GetAssessmentsBySubjectId(Guid subjectId)
        {
            return _assessments.Where(a => a.SubjectId == subjectId);
        }
    }
}
