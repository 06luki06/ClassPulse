using System;
using System.Collections.Generic;

namespace At.luki0606.ClassPulse.Data.Entities
{
    public class Subject
    {
        public Guid Id { get; private set; }

        public string Name { get; private set; }
        public string Code { get; private set; }

        private readonly List<Assessment> _assessments = [];
        public IReadOnlyCollection<Assessment> Assessments => _assessments.AsReadOnly();

        private readonly List<SubjectNote> _subjectNotes = [];
        public IReadOnlyCollection<SubjectNote> SubjectNotes => _subjectNotes.AsReadOnly();

        private Subject()
        {
            Name = string.Empty;
            Code = string.Empty;
        }

        public Subject(string name, string code)
        {
            ValidateName(name);
            ValidateCode(code);

            Id = Guid.NewGuid();
            Name = name.Trim();
            Code = code.Trim().ToUpperInvariant();
        }

        public void Update(string name, string code)
        {
            ValidateName(name);
            ValidateCode(code);

            Name = name.Trim();
            Code = code.Trim().ToUpperInvariant();
        }

        private static void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }
        }

        private static void ValidateCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                throw new ArgumentNullException(nameof(code));
            }
        }
    }
}
