using At.luki0606.ClassPulse.Data.Entities;
using At.luki0606.ClassPulse.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace At.luki0606.ClassPulse.ViewModels
{
    public partial class StudentDetailViewModel : ViewModelBase
    {
        private readonly IClassService _classService;
        private readonly IAssessmentService _assessmentService;
        private readonly ClassDetailViewModel _parentClassDetailVm;

        public Guid StudentId { get; }

        [ObservableProperty]
        private Student? _currentStudent;

        [ObservableProperty]
        private string _studentName = string.Empty;

        public ObservableCollection<SubjectDetailGroup> SubjectGroups { get; } = [];

        public StudentDetailViewModel(
            Guid studentId,
            ClassDetailViewModel parentClassDetailVm,
            IClassService classService,
            IAssessmentService assessmentService
        )
        {
            StudentId = studentId;
            _parentClassDetailVm = parentClassDetailVm;
            _classService = classService;
            _assessmentService = assessmentService;

            _ = LoadStudentDataAsync();
        }

        private async Task LoadStudentDataAsync()
        {
            SubjectGroups.Clear();
            CurrentStudent = await _classService.GetStudentDetailsAsync(StudentId);

            if (CurrentStudent == null)
            {
                return;
            }

            StudentName = CurrentStudent.FullName;
            List<Subject> allSubjects = await _classService.GetAllSubjectsAsync();

            foreach (Subject subject in allSubjects)
            {
                List<Assessment> assessments = [.. CurrentStudent.GetAssessmentsBySubjectId(subject.Id)];
                double avg = _assessmentService.CalculateSubjectAverage(assessments);
                bool hasWarning = _assessmentService.HasPerformanceDrop(assessments);

                SubjectGroups.Add(new SubjectDetailGroup(
                    subject.Name,
                    subject.Code,
                    avg > 0 ? avg.ToString("0.0") : "-",
                    hasWarning,
                    assessments
                ));
            }
        }

        [RelayCommand]
        private void BackToClassDetail()
        {
            if (App.Current is App { Services: { } services })
            {
                MainWindowViewModel mainVm = services.GetRequiredService<MainWindowViewModel>();
                mainVm.NavigateToClassDetail(_parentClassDetailVm);
            }
        }
    }



    public record SubjectDetailGroup(
        string SubjectName,
        string SubjectCode,
        string Average,
        bool HasPerformanceDropWarning,
        List<Assessment> Assessments
    );
}
