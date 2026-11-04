using At.luki0606.ClassPulse.Data.Entities;
using At.luki0606.ClassPulse.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace At.luki0606.ClassPulse.ViewModels.Dialogs
{
    /// <summary>
    /// View model of the grade editor dialog.
    /// </summary>
    public partial class AssessmentGradeEditViewModel : ViewModelBase
    {
        private readonly IAssessmentService _assessmentService;

        /// <summary>
        /// Gets the dialog title.
        /// </summary>
        public string Title { get; }
        /// <summary>
        /// Gets one editable entry per result.
        /// </summary>
        public ObservableCollection<AssessmentGradeEntryDto> GradeEntries { get; } = [];

        /// <summary>
        /// Gets a value indicating whether the entries were saved.
        /// </summary>
        public bool IsConfirmed { get; private set; }
        /// <summary>
        /// Occurs when the dialog should close.
        /// </summary>
        public event Action? OnCloseRequested;

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="title">The dialog title.</param>
        /// <param name="assessments">The results to edit.</param>
        /// <param name="assessmentService">The service that saves the changes.</param>
        public AssessmentGradeEditViewModel(string title, List<Assessment> assessments, IAssessmentService assessmentService)
        {
            Title = title;
            _assessmentService = assessmentService;

            foreach (Assessment assessment in assessments)
            {
                GradeEntries.Add(new AssessmentGradeEntryDto(
                    assessment.Id,
                    assessment.Student?.FullName ?? "-",
                    assessment.Grade,
                    assessment.PositiveFeedback,
                    assessment.ImprovementNotes,
                    assessment.HasParticipated
                ));
            }
        }

        [RelayCommand]
        private async Task SaveAsync()
        {
            foreach (AssessmentGradeEntryDto entry in GradeEntries)
            {
                await _assessmentService.UpdateAssessmentGradeAsync(
                    entry.AssessmentId,
                    entry.Grade,
                    entry.PositiveFeedback,
                    entry.ImprovementNotes,
                    entry.HasParticipated
                );
            }

            IsConfirmed = true;
            OnCloseRequested?.Invoke();
        }

        [RelayCommand]
        private void Cancel()
        {
            IsConfirmed = false;
            OnCloseRequested?.Invoke();
        }
    }

    /// <summary>
    /// An editable result. Participation and grade stay in sync: non-participation means grade 0.0, and participation means a grade of at least 1.0.
    /// </summary>
    public partial class AssessmentGradeEntryDto : ViewModelBase
    {
        /// <summary>
        /// Gets the identifier of the result.
        /// </summary>
        public Guid AssessmentId { get; }
        /// <summary>
        /// Gets the student's full name.
        /// </summary>
        public string StudentName { get; }

        /// <summary>
        /// Whether the student participated.
        /// </summary>
        [ObservableProperty]
        private bool _hasParticipated;

        /// <summary>
        /// The grade from 1.0 to 5.0, or 0.0 for non-participation.
        /// </summary>
        [ObservableProperty]
        private double _grade;

        /// <summary>
        /// The optional positive feedback.
        /// </summary>
        [ObservableProperty]
        private string? _positiveFeedback;

        /// <summary>
        /// The optional improvement notes.
        /// </summary>
        [ObservableProperty]
        private string? _improvementNotes;

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="assessmentId">The identifier of the result.</param>
        /// <param name="studentName">The student's full name.</param>
        /// <param name="grade">The current grade.</param>
        /// <param name="positiveFeedback">The current positive feedback.</param>
        /// <param name="improvementNotes">The current improvement notes.</param>
        /// <param name="hasParticipated">Whether the student participated.</param>
        public AssessmentGradeEntryDto(
            Guid assessmentId,
            string studentName,
            double grade,
            string? positiveFeedback,
            string? improvementNotes,
            bool hasParticipated = true)
        {
            AssessmentId = assessmentId;
            StudentName = studentName;
            _grade = grade;
            _positiveFeedback = positiveFeedback;
            _improvementNotes = improvementNotes;
            _hasParticipated = hasParticipated;
        }

        partial void OnHasParticipatedChanged(bool value)
        {
            if (value && Grade < 1.0)
            {
                Grade = 1.0;
            }
            else if (!value && Grade > 0.0)
            {
                Grade = 0.0;
            }
        }

        partial void OnGradeChanged(double value)
        {
            if (value <= 0.0 && HasParticipated)
            {
                HasParticipated = false;
            }
            else if (value >= 1.0 && !HasParticipated)
            {
                HasParticipated = true;
            }
        }
    }
}
