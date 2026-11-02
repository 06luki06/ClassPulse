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
    public partial class AssessmentGradeEditViewModel : ViewModelBase
    {
        private readonly IAssessmentService _assessmentService;

        public string Title { get; }
        public ObservableCollection<AssessmentGradeEntryDto> GradeEntries { get; } = [];

        public bool IsConfirmed { get; private set; }
        public event Action? OnCloseRequested;

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

    public partial class AssessmentGradeEntryDto : ViewModelBase
    {
        public Guid AssessmentId { get; }
        public string StudentName { get; }

        [ObservableProperty]
        private bool _hasParticipated;

        [ObservableProperty]
        private double _grade;

        [ObservableProperty]
        private string? _positiveFeedback;

        [ObservableProperty]
        private string? _improvementNotes;

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
