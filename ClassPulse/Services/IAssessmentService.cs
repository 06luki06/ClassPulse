using At.luki0606.ClassPulse.Data.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace At.luki0606.ClassPulse.Services
{
    /// <summary>
    /// Creates and changes assessment results and calculates grade statistics.
    /// </summary>
    public interface IAssessmentService
    {
        /// <summary>
        /// Creates one result for every student currently in a class.
        /// </summary>
        /// <param name="schoolClassId">The identifier of the class.</param>
        /// <param name="subjectId">The identifier of the subject.</param>
        /// <param name="title">The title. It must not be blank.</param>
        /// <param name="assessmentDate">The date of the assessment.</param>
        /// <param name="weight">The weight. It must be greater than zero.</param>
        /// <param name="defaultGrade">The grade stored for every student until it is edited.</param>
        /// <returns>The created results.</returns>
        /// <exception cref="InvalidOperationException">The class has no students.</exception>
        Task<List<Assessment>> CreateClassAssessmentAsync(
            Guid schoolClassId,
            Guid subjectId,
            string title,
            DateTime assessmentDate,
            double weight = 1.0,
            double defaultGrade = 1.0);

        /// <summary>
        /// Updates the grade, feedback and participation of a result.
        /// </summary>
        /// <param name="assessmentId">The identifier of the result.</param>
        /// <param name="grade">The grade from 1.0 to 5.0, or 0.0 for non-participation.</param>
        /// <param name="positiveFeedback">The optional positive feedback.</param>
        /// <param name="improvementNotes">The optional improvement notes.</param>
        /// <param name="hasParticipated">Whether the student participated.</param>
        /// <returns>A task that completes when the change is saved.</returns>
        /// <exception cref="InvalidOperationException">The result does not exist.</exception>
        Task UpdateAssessmentGradeAsync(
            Guid assessmentId,
            double grade,
            string? positiveFeedback = null,
            string? improvementNotes = null,
            bool hasParticipated = true);

        /// <summary>
        /// Adds a note about a student in a subject.
        /// </summary>
        /// <param name="studentId">The identifier of the student.</param>
        /// <param name="subjectId">The identifier of the subject.</param>
        /// <param name="noteText">The note text. It must not be blank.</param>
        /// <returns>The created note.</returns>
        Task<SubjectNote> AddSubjectNoteAsync(Guid studentId, Guid subjectId, string noteText);

        /// <summary>
        /// Calculates the weighted average of the results the student participated in.
        /// </summary>
        /// <param name="assessments">The results to include.</param>
        /// <returns>The average rounded to two decimal places, or 0.0 if no result counts.</returns>
        double CalculateSubjectAverage(IEnumerable<Assessment> assessments);

        /// <summary>
        /// Detects a recent decline in grades. Higher grades are worse.
        /// </summary>
        /// <param name="assessments">The results to examine.</param>
        /// <returns><see langword="true"/> if at least three results count and the average of the latest two is at least 0.75 above the overall average.</returns>
        bool HasPerformanceDrop(IEnumerable<Assessment> assessments);

        /// <summary>
        /// Gets the results of one class assessment, sorted by student name.
        /// </summary>
        /// <param name="schoolClassId">The identifier of the class.</param>
        /// <param name="subjectId">The identifier of the subject.</param>
        /// <param name="title">The exact title of the assessment.</param>
        /// <returns>The matching results.</returns>
        Task<List<Assessment>> GetAssessmentsByTitleAndSubjectAsync(Guid schoolClassId, Guid subjectId, string title);
    }
}
