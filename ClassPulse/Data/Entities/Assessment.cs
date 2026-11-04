using System;

namespace At.luki0606.ClassPulse.Data.Entities
{
    public class Assessment
    {
        public Guid Id { get; private set; }
        public string Title { get; private set; }
        public DateTime Date { get; private set; }

        public double Grade { get; private set; }
        public double Weight { get; private set; }
        public bool HasParticipated { get; private set; } = true;

        public string? PositiveFeedback { get; private set; }
        public string? ImprovementNotes { get; private set; }

        public Guid StudentId { get; private set; }
        public Student? Student { get; private set; }

        public Guid SubjectId { get; private set; }
        public Subject? Subject { get; private set; }

        private Assessment()
        {
            Title = string.Empty;
        }

        public Assessment(
            string title,
            DateTime date,
            double grade,
            Guid studentId,
            Guid subjectId,
            double weight = 1.0,
            string? positiveFeedback = null,
            string? improvementNotes = null,
            bool hasParticipated = true)
        {
            if (grade == 0.0)
            {
                hasParticipated = false;
            }

            ValidateTitle(title);
            ValidateGrade(grade, hasParticipated);
            ValidateWeight(weight);
            ValidateId(studentId);
            ValidateId(subjectId);

            Id = Guid.NewGuid();
            Title = title;
            Date = date;
            Grade = grade;
            Weight = weight;
            PositiveFeedback = positiveFeedback?.Trim();
            ImprovementNotes = improvementNotes?.Trim();
            StudentId = studentId;
            SubjectId = subjectId;
            HasParticipated = hasParticipated;
        }

        public void Update(
            string title,
            DateTime date,
            double grade,
            double weight = 1.0,
            string? positiveFeedback = null,
            string? improvementNotes = null,
            bool hasParticipated = true)
        {
            if (grade == 0.0)
            {
                hasParticipated = false;
            }

            ValidateTitle(title);
            ValidateGrade(grade, hasParticipated);
            ValidateWeight(weight);
            Title = title;
            Date = date;
            Grade = grade;
            Weight = weight;
            PositiveFeedback = positiveFeedback?.Trim();
            ImprovementNotes = improvementNotes?.Trim();
            HasParticipated = hasParticipated;
        }

        private static void ValidateTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentNullException(nameof(title));
            }
        }

        private static void ValidateGrade(double grade, bool hasParticipated = true)
        {
            if (!hasParticipated && grade == 0.0)
            {
                return;
            }

            if (grade is < 1.0 or > 5.0)
            {
                throw new ArgumentOutOfRangeException(nameof(grade));
            }
        }

        private static void ValidateWeight(double weight)
        {
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(weight, 0.0);
        }

        private static void ValidateId(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException(null, nameof(id));
            }
        }
    }
}
