using At.luki0606.ClassPulse.Data.Entities;
using At.luki0606.ClassPulse.Services;

namespace At.luki0606.ClassPulse.Tests.Stubs
{
    internal class AssessmentServiceStub : IAssessmentService
    {
        private readonly List<Assessment> _assessments = [];
        private readonly List<SubjectNote> _subjectNotes = [];

        private readonly Dictionary<Guid, Guid> _studentToClassMap = [];

        public void RegisterStudentToClass(Guid studentId, Guid schoolClassId)
        {
            _studentToClassMap[studentId] = schoolClassId;
        }

        public Task<SubjectNote> AddSubjectNoteAsync(Guid studentId, Guid subjectId, string noteText)
        {
            SubjectNote subjectNote = new(noteText, studentId, subjectId);
            _subjectNotes.Add(subjectNote);
            return Task.FromResult(subjectNote);
        }

        public double CalculateSubjectAverage(IEnumerable<Assessment> assessments)
        {
            List<Assessment> list = assessments.ToList();
            if (list.Count == 0)
            {
                return 0.0;
            }

            double totalWeightedGrades = list.Sum(a => a.Grade * a.Weight);
            double totalWeight = list.Sum(a => a.Weight);

            return totalWeight > 0 ? Math.Round(totalWeightedGrades / totalWeight, 2) : 0.0;
        }

        public Task<List<Assessment>> CreateClassAssessmentAsync(
            Guid schoolClassId,
            Guid subjectId,
            string title,
            DateTime date,
            double weight = 1,
            double defaultGrade = 1)
        {
            List<Guid> studentIds = [.. _studentToClassMap
                .Where(kvp => kvp.Value == schoolClassId)
                .Select(kvp => kvp.Key)];

            if (studentIds.Count == 0)
            {
                throw new InvalidOperationException("No students found in the specified class.");
            }

            List<Assessment> newAssessments = [];

            foreach (Guid studentId in studentIds)
            {
                Assessment assessment = new(
                    title: title,
                    date: date,
                    grade: defaultGrade,
                    studentId: studentId,
                    subjectId: subjectId,
                    weight: weight
                );
                newAssessments.Add(assessment);
            }

            _assessments.AddRange(newAssessments);
            return Task.FromResult(newAssessments);
        }

        public bool HasPerformanceDrop(IEnumerable<Assessment> assessments)
        {
            List<Assessment> sorted = assessments.OrderByDescending(a => a.Date).ToList();
            if (sorted.Count < 3)
            {
                return false;
            }

            double recentAvg = sorted.Take(2).Average(a => a.Grade);
            double overallAvg = sorted.Average(a => a.Grade);

            return (recentAvg - overallAvg) >= 0.75;
        }

        public Task UpdateAssessmentGradeAsync(
            Guid assessmentId,
            double grade,
            string? positiveFeedback = null,
            string? improvementNotes = null)
        {
            Assessment assessment = _assessments.FirstOrDefault(a => a.Id == assessmentId)
                ?? throw new InvalidOperationException("Assessment not found.");

            assessment.Update(
                title: assessment.Title,
                date: assessment.Date,
                grade: grade,
                weight: assessment.Weight,
                positiveFeedback: positiveFeedback,
                improvementNotes: improvementNotes
            );

            return Task.CompletedTask;
        }
    }
}