using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace At.luki0606.ClassPulse.Data.Entities
{
    public partial class SchoolClass
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string SchoolYear { get; private set; }

        [GeneratedRegex(@"^\d{4}/\d{4}$")]
        private static partial Regex SchoolYearRegex();

        private readonly List<Student> _students = [];
        public IReadOnlyCollection<Student> Students => _students.AsReadOnly();

        private SchoolClass()
        {
            Name = string.Empty;
            SchoolYear = string.Empty;
        }

        public SchoolClass(string name, string schoolYear)
        {
            ValidateName(name);
            ValidateSchoolYear(schoolYear);
            Id = Guid.NewGuid();
            Name = name.Trim();
            SchoolYear = schoolYear;
        }

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
            if (schoolYear == null || !SchoolYearRegex().IsMatch(schoolYear))
            {
                throw new ArgumentException("Invalid school year format.", nameof(schoolYear));
            }
        }
    }
}
