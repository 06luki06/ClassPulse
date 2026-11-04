using System;
using System.Collections.Generic;

namespace At.luki0606.ClassPulse.Data.Entities
{
    /// <summary>
    /// A school subject.
    /// </summary>
    public class Subject
    {
        /// <summary>
        /// Gets the unique identifier.
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Gets the full name.
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Gets the short code in upper case, for example <c>M</c>.
        /// </summary>
        public string Code { get; private set; }

        private readonly List<Assessment> _assessments = [];
        /// <summary>
        /// Gets the assessment results in this subject, if loaded.
        /// </summary>
        public IReadOnlyCollection<Assessment> Assessments => _assessments.AsReadOnly();

        private readonly List<SubjectNote> _subjectNotes = [];
        /// <summary>
        /// Gets the notes in this subject, if loaded.
        /// </summary>
        public IReadOnlyCollection<SubjectNote> SubjectNotes => _subjectNotes.AsReadOnly();

        private Subject()
        {
            Name = string.Empty;
            Code = string.Empty;
        }

        /// <summary>
        /// Initializes a new subject.
        /// </summary>
        /// <param name="name">The name. It must not be blank and is trimmed.</param>
        /// <param name="code">The short code. It must not be blank, is trimmed and is converted to upper case.</param>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> or <paramref name="code"/> is blank.</exception>
        public Subject(string name, string code)
        {
            ValidateName(name);
            ValidateCode(code);

            Id = Guid.NewGuid();
            Name = name.Trim();
            Code = code.Trim().ToUpperInvariant();
        }

        /// <summary>
        /// Replaces the name and short code.
        /// </summary>
        /// <param name="name">The name. It must not be blank and is trimmed.</param>
        /// <param name="code">The short code. It must not be blank, is trimmed and is converted to upper case.</param>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> or <paramref name="code"/> is blank.</exception>
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
