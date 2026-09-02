using At.luki0606.ClassPulse.Data;
using At.luki0606.ClassPulse.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace At.luki0606.ClassPulse.Services
{
    public class AssessmentService : IAssessmentService
    {
        private readonly AppDbContext _dbContext;

        public AssessmentService(AppDbContext appDbContext)
        {
            _dbContext = appDbContext;
        }

        public async Task<SubjectNote> AddSubjectNoteAsync(Guid studentId, Guid subjectId, string noteText)
        {
            SubjectNote subjectNote = new(noteText, studentId, subjectId);
            _dbContext.SubjectNotes.Add(subjectNote);
            await _dbContext.SaveChangesAsync();
            return subjectNote;
        }

        public double CalculateSubjectAverage(IEnumerable<Assessment> assessments)
        {
            List<Assessment> list = assessments as List<Assessment> ?? [.. assessments];
            if (list.Count == 0)
            {
                return 0.0;
            }

            double totalWeight = list.Sum(a => a.Weight);
            if (totalWeight <= 0)
            {
                return 0.0;
            }

            double totalWeightedGrades = list.Sum(a => a.Grade * a.Weight);
            return Math.Round(totalWeightedGrades / totalWeight, 2);
        }

        public async Task<List<Assessment>> CreateClassAssessmentAsync(Guid schoolClassId, Guid subjectId, string title, DateTime date, double weight = 1, double defaultGrade = 1)
        {
            List<Guid> studentIds = await _dbContext.Students
                .Where(s => s.SchoolClassId == schoolClassId)
                .Select(s => s.Id)
                .ToListAsync();

            if (studentIds.Count == 0)
            {
                throw new InvalidOperationException("No students found in the specified class.");
            }

            List<Assessment> newAssessments = [.. studentIds
                .Select(studentId => new Assessment(
                    title: title,
                    date: date,
                    grade: defaultGrade,
                    studentId: studentId,
                    subjectId: subjectId,
                    weight: weight
                ))];

            await _dbContext.Assessments.AddRangeAsync(newAssessments);
            await _dbContext.SaveChangesAsync();

            return newAssessments;
        }

        public bool HasPerformanceDrop(IEnumerable<Assessment> assessments)
        {
            List<Assessment> sorted = [.. assessments.OrderByDescending(a => a.Date)];
            if (sorted.Count < 3)
            {
                return false;
            }

            double recentAvg = sorted.Take(2).Average(a => a.Grade);
            double overallAvg = sorted.Average(a => a.Grade);

            return (recentAvg - overallAvg) >= 0.75;
        }

        public async Task UpdateAssessmentGradeAsync(Guid assessmentId, double grade, string? positiveFeedback = null, string? improvementNotes = null)
        {
            Assessment assessment = await _dbContext.Assessments.FindAsync(assessmentId)
                ?? throw new InvalidOperationException("Assessment not found.");

            assessment.Update(
                title: assessment.Title,
                date: assessment.Date,
                grade: grade,
                weight: assessment.Weight,
                positiveFeedback: positiveFeedback,
                improvementNotes: improvementNotes
            );
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Assessment>> GetAssessmentsByTitleAndSubjectAsync(Guid schoolClassId, Guid subjectId, string title)
        {
            return await _dbContext.Assessments
                .Where(a => a.SubjectId == subjectId &&
                            a.Title == title &&
                            a.Student != null &&
                            a.Student.SchoolClassId == schoolClassId)
                .OrderBy(a => a.Student!.LastName)
                .ThenBy(a => a.Student!.FirstName)
                .ToListAsync();
        }
    }
}