using At.luki0606.ClassPulse.Data.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace At.luki0606.ClassPulse.Services
{
    public interface IAssessmentService
    {
        Task<List<Assessment>> CreateClassAssessmentAsync(
            Guid schoolClassId,
            Guid subjectId,
            string title,
            DateTime date,
            double weight = 1.0,
            double defaultGrade = 1.0);

        Task UpdateAssessmentGradeAsync(
            Guid assessmentId,
            double grade,
            string? positiveFeedback = null,
            string? improvementNotes = null);

        Task<SubjectNote> AddSubjectNoteAsync(Guid studentId, Guid subjectId, string noteText);

        double CalculateSubjectAverage(IEnumerable<Assessment> assessments);

        bool HasPerformanceDrop(IEnumerable<Assessment> assessments);
    }
}
