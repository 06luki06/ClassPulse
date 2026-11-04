using System;

namespace At.luki0606.ClassPulse.Data.Entities
{
    /// <summary>
    /// One student's result for an assessment in a subject.
    /// </summary>
    public class Assessment
    {
        /// <summary>
        /// Gets the unique identifier.
        /// </summary>
        public Guid Id { get; private set; }
        /// <summary>
        /// Gets the title. It is shared by all results of the same class assessment.
        /// </summary>
        public string Title { get; private set; }
        /// <summary>
        /// Gets the date of the assessment.
        /// </summary>
        public DateTime Date { get; private set; }

        /// <summary>
        /// Gets the grade from 1.0 (best) to 5.0 (worst). The value 0.0 means the student did not participate.
        /// </summary>
        public double Grade { get; private set; }
        /// <summary>
        /// Gets the weight of this result in averages. It is always greater than zero.
        /// </summary>
        public double Weight { get; private set; }
        /// <summary>
        /// Gets a value indicating whether the student participated. Non-participating results are excluded from averages.
        /// </summary>
        public bool HasParticipated { get; private set; } = true;

        /// <summary>
        /// Gets the optional positive feedback.
        /// </summary>
        public string? PositiveFeedback { get; private set; }
        /// <summary>
        /// Gets the optional notes on what to improve.
        /// </summary>
        public string? ImprovementNotes { get; private set; }

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

        private Assessment()
        {
            Title = string.Empty;
        }

        /// <summary>
        /// Initializes a new result. A grade of 0.0 always sets <see cref="HasParticipated"/> to <see langword="false"/>.
        /// </summary>
        /// <param name="title">The title. It must not be blank.</param>
        /// <param name="date">The date of the assessment.</param>
        /// <param name="grade">The grade from 1.0 to 5.0, or 0.0 for non-participation.</param>
        /// <param name="studentId">The identifier of the student. It must not be empty.</param>
        /// <param name="subjectId">The identifier of the subject. It must not be empty.</param>
        /// <param name="weight">The weight. It must be greater than zero.</param>
        /// <param name="positiveFeedback">The optional positive feedback. It is trimmed.</param>
        /// <param name="improvementNotes">The optional improvement notes. They are trimmed.</param>
        /// <param name="hasParticipated">Whether the student participated.</param>
        /// <exception cref="ArgumentNullException"><paramref name="title"/> is blank.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="grade"/> or <paramref name="weight"/> is out of range.</exception>
        /// <exception cref="ArgumentException"><paramref name="studentId"/> or <paramref name="subjectId"/> is empty.</exception>
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

        /// <summary>
        /// Replaces the editable values. A grade of 0.0 always sets <see cref="HasParticipated"/> to <see langword="false"/>.
        /// </summary>
        /// <param name="title">The title. It must not be blank.</param>
        /// <param name="date">The date of the assessment.</param>
        /// <param name="grade">The grade from 1.0 to 5.0, or 0.0 for non-participation.</param>
        /// <param name="weight">The weight. It must be greater than zero.</param>
        /// <param name="positiveFeedback">The optional positive feedback. It is trimmed.</param>
        /// <param name="improvementNotes">The optional improvement notes. They are trimmed.</param>
        /// <param name="hasParticipated">Whether the student participated.</param>
        /// <exception cref="ArgumentNullException"><paramref name="title"/> is blank.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="grade"/> or <paramref name="weight"/> is out of range.</exception>
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
