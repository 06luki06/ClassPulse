using At.luki0606.ClassPulse.Data.Entities;
using At.luki0606.ClassPulse.Services;
using At.luki0606.ClassPulse.ViewModels.Dialogs;
using At.luki0606.ClassPulse.Views.Dialogs;
using Avalonia.Controls.ApplicationLifetimes;
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
        private async Task BackToClassDetail()
        {
            if (App.Current is App { Services: { } services })
            {
                await _parentClassDetailVm.LoadDataAsync();

                MainWindowViewModel mainVm = services.GetRequiredService<MainWindowViewModel>();
                mainVm.NavigateToClassDetail(_parentClassDetailVm);
            }
        }

        [RelayCommand]
        private async Task EditAssessmentGradeAsync(Assessment assessment)
        {
            if (App.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop && desktop.MainWindow != null)
            {
                List<Assessment> list = [assessment];
                AssessmentGradeEditViewModel vm = new(assessment.Title, list, _assessmentService);
                AssessmentGradeEditWindow window = new() { DataContext = vm };

                vm.OnCloseRequested += () => window.Close();
                await window.ShowDialog(desktop.MainWindow);

                if (vm.IsConfirmed)
                {
                    await LoadStudentDataAsync();
                }
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
