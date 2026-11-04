using System;
using System.Collections.Generic;
using System.Linq;

namespace At.luki0606.ClassPulse.Data.Entities
{
    /// <summary>
    /// A student who belongs to one school class.
    /// </summary>
    public class Student
    {
        /// <summary>
        /// Gets the unique identifier.
        /// </summary>
        public Guid Id { get; private set; }
        /// <summary>
        /// Gets the first name.
        /// </summary>
        public string? FirstName { get; private set; }
        /// <summary>
        /// Gets the last name.
        /// </summary>
        public string? LastName { get; private set; }
        /// <summary>
        /// Gets the optional general notes about the student.
        /// </summary>
        public string? GeneralNotes { get; private set; }

        /// <summary>
        /// Gets the identifier of the school class.
        /// </summary>
        public Guid SchoolClassId { get; private set; }
        /// <summary>
        /// Gets the school class, if loaded.
        /// </summary>
        public SchoolClass? SchoolClass { get; private set; }

        private readonly List<SubjectNote> _subjectNotes = [];
        /// <summary>
        /// Gets the subject notes, if loaded.
        /// </summary>
        public IReadOnlyCollection<SubjectNote> SubjectNotes => _subjectNotes.AsReadOnly();

        private readonly List<Assessment> _assessments = [];
        /// <summary>
        /// Gets the assessment results, if loaded.
        /// </summary>
        public IReadOnlyCollection<Assessment> Assessments => _assessments.AsReadOnly();

        /// <summary>
        /// Gets the first and last name, separated by a space.
        /// </summary>
        public string FullName => $"{FirstName} {LastName}";

        private Student()
        {
            FirstName = string.Empty;
            LastName = string.Empty;
        }

        /// <summary>
        /// Initializes a new student.
        /// </summary>
        /// <param name="firstName">The first name. It must not be blank and is trimmed.</param>
        /// <param name="lastName">The last name. It must not be blank and is trimmed.</param>
        /// <param name="schoolClassId">The identifier of the school class. It must not be empty.</param>
        /// <param name="generalNotes">The optional general notes. They are trimmed.</param>
        /// <exception cref="ArgumentNullException">A name is blank or <paramref name="schoolClassId"/> is empty.</exception>
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

        /// <summary>
        /// Replaces the names, class and general notes.
        /// </summary>
        /// <param name="firstName">The first name. It must not be blank and is trimmed.</param>
        /// <param name="lastName">The last name. It must not be blank and is trimmed.</param>
        /// <param name="schoolClassId">The identifier of the school class. It must not be empty.</param>
        /// <param name="generalNotes">The optional general notes. They are trimmed.</param>
        /// <exception cref="ArgumentNullException">A name is blank or <paramref name="schoolClassId"/> is empty.</exception>
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

        /// <summary>
        /// Replaces the general notes.
        /// </summary>
        /// <param name="generalNotes">The new notes, or <see langword="null"/> to clear them. They are trimmed.</param>
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

        /// <summary>
        /// Filters the loaded assessment results by subject.
        /// </summary>
        /// <param name="subjectId">The identifier of the subject.</param>
        /// <returns>The student's results in that subject.</returns>
        public IEnumerable<Assessment> GetAssessmentsBySubjectId(Guid subjectId)
        {
            return _assessments.Where(a => a.SubjectId == subjectId);
        }
    }
}
