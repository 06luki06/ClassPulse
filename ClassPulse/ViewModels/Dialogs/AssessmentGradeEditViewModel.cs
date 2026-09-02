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
                    assessment.ImprovementNotes
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
                    entry.ImprovementNotes
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
        private double _grade;

        [ObservableProperty]
        private string? _positiveFeedback;

        [ObservableProperty]
        private string? _improvementNotes;

        public AssessmentGradeEntryDto(Guid assessmentId, string studentName, double grade, string? positiveFeedback, string? improvmentNotes)
        {
            AssessmentId = assessmentId;
            StudentName = studentName;
            _grade = grade;
            _positiveFeedback = positiveFeedback;
            _improvementNotes = improvmentNotes;
        }
    }
}
