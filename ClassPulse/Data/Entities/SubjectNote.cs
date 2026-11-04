using System;

namespace At.luki0606.ClassPulse.Data.Entities
{
    /// <summary>
    /// A dated note about a student in a subject.
    /// </summary>
    public class SubjectNote
    {
        /// <summary>
        /// Gets the unique identifier.
        /// </summary>
        public Guid Id { get; private set; }
        /// <summary>
        /// Gets the note text.
        /// </summary>
        public string? Text { get; private set; }
        /// <summary>
        /// Gets the local time at which the note was created.
        /// </summary>
        public DateTime CreatedAt { get; private set; }

        /// <summary>
        /// Gets the identifier of the student.
        /// </summary>
        public Guid StudentId { get; private set; }
        /// <summary>
        /// Gets the student, if loaded.
        /// </summary>
        public Student? Student { get; private set; }

        /// <summary>
        /// Gets the identifier of the subject.
        /// </summary>
        public Guid SubjectId { get; private set; }
        /// <summary>
        /// Gets the subject, if loaded.
        /// </summary>
        public Subject? Subject { get; private set; }

        private SubjectNote()
        {
            Text = string.Empty;
        }

        /// <summary>
        /// Initializes a new note with the current local time.
        /// </summary>
        /// <param name="text">The note text. It must not be blank and is trimmed.</param>
        /// <param name="studentId">The identifier of the student. It must not be empty.</param>
        /// <param name="subjectId">The identifier of the subject. It must not be empty.</param>
        /// <exception cref="ArgumentNullException"><paramref name="text"/> is blank.</exception>
        /// <exception cref="ArgumentException"><paramref name="studentId"/> or <paramref name="subjectId"/> is empty.</exception>
        public SubjectNote(string text, Guid studentId, Guid subjectId)
        {
            ValidateText(text);
            ValidateId(studentId);
            ValidateId(subjectId);
            Id = Guid.NewGuid();
            Text = text.Trim();
            CreatedAt = DateTime.Now;
            StudentId = studentId;
            SubjectId = subjectId;
        }

        /// <summary>
        /// Replaces the note text.
        /// </summary>
        /// <param name="text">The note text. It must not be blank and is trimmed.</param>
        /// <exception cref="ArgumentNullException"><paramref name="text"/> is blank.</exception>
        public void UpdateText(string text)
        {
            ValidateText(text);
            Text = text.Trim();
        }

        private static void ValidateText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentNullException(nameof(text));
            }
        }

        private static void ValidateId(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("Id cannot be empty.", nameof(id));
            }
        }
    }
}
