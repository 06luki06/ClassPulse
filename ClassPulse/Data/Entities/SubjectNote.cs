using System;

namespace At.luki0606.ClassPulse.Data.Entities
{
    public class SubjectNote
    {
        public Guid Id { get; private set; }
        public string? Text { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public Guid StudentId { get; private set; }
        public Student? Student { get; private set; } = null;

        public Guid SubjectId { get; private set; }
        public Subject? Subject { get; private set; } = null;

        private SubjectNote()
        {
            Text = string.Empty;
        }

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
