using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace At.luki0606.ClassPulse.Data.Entities
{
    /// <summary>
    /// A school class in a specific school year.
    /// </summary>
    public class SchoolClass
    {
        /// <summary>
        /// Gets the unique identifier.
        /// </summary>
        public Guid Id { get; private set; }
        /// <summary>
        /// Gets the class name, for example <c>1a</c>.
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Gets the school year in the format <c>YYYY/YYYY</c>.
        /// </summary>
        public string SchoolYear { get; private set; }

        private static readonly Regex _schoolYearRegex = new(@"^\d{4}/\d{4}$");

        private readonly List<Student> _students = [];
        /// <summary>
        /// Gets the students of this class, if loaded.
        /// </summary>
        public IReadOnlyCollection<Student> Students => _students.AsReadOnly();

        private SchoolClass()
        {
            Name = string.Empty;
            SchoolYear = string.Empty;
        }

        /// <summary>
        /// Initializes a new class.
        /// </summary>
        /// <param name="name">The class name. It must not be blank and is trimmed.</param>
        /// <param name="schoolYear">The school year in the format <c>YYYY/YYYY</c>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is blank.</exception>
        /// <exception cref="ArgumentException"><paramref name="schoolYear"/> has an invalid format.</exception>
        public SchoolClass(string name, string schoolYear)
        {
            ValidateName(name);
            ValidateSchoolYear(schoolYear);
            Id = Guid.NewGuid();
            Name = name.Trim();
            SchoolYear = schoolYear;
        }

        /// <summary>
        /// Replaces the name and school year.
        /// </summary>
        /// <param name="name">The class name. It must not be blank and is trimmed.</param>
        /// <param name="schoolYear">The school year in the format <c>YYYY/YYYY</c>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is blank.</exception>
        /// <exception cref="ArgumentException"><paramref name="schoolYear"/> has an invalid format.</exception>
        public void Update(string name, string schoolYear)
        {
            ValidateName(name);
            ValidateSchoolYear(schoolYear);
            Name = name.Trim();
            SchoolYear = schoolYear;
        }

        private static void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }
        }

        private static void ValidateSchoolYear(string schoolYear)
        {
            if (schoolYear == null || !_schoolYearRegex.IsMatch(schoolYear))
            {
                throw new ArgumentException("Invalid school year format.", nameof(schoolYear));
            }
        }
    }
}
