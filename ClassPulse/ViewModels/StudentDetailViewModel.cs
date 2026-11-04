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
using System.Globalization;
using System.Threading.Tasks;

namespace At.luki0606.ClassPulse.ViewModels
{
    /// <summary>
    /// View model of the student page, which shows results per subject and opens the grade editor.
    /// </summary>
    public partial class StudentDetailViewModel : ViewModelBase
    {
        private readonly IClassService _classService;
        private readonly IAssessmentService _assessmentService;
        private readonly ClassDetailViewModel _parentClassDetailVm;

        /// <summary>
        /// Gets the identifier of the student shown on this page.
        /// </summary>
        public Guid StudentId { get; }

        /// <summary>
        /// The loaded student, or <see langword="null"/> if the student was not found or is still loading.
        /// </summary>
        [ObservableProperty]
        private Student? _currentStudent;

        /// <summary>
        /// The student's full name for display.
        /// </summary>
        [ObservableProperty]
        private string _studentName = string.Empty;

        /// <summary>
        /// Gets one group per subject.
        /// </summary>
        public ObservableCollection<SubjectDetailGroup> SubjectGroups { get; } = [];

        /// <summary>
        /// Initializes a new instance and starts loading the data in the background.
        /// </summary>
        /// <param name="studentId">The identifier of the student.</param>
        /// <param name="parentClassDetailVm">The class page to return to.</param>
        /// <param name="classService">The class service.</param>
        /// <param name="assessmentService">The assessment service.</param>
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
                    avg > 0 ? avg.ToString("0.0", CultureInfo.CurrentCulture) : "-",
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



    /// <summary>
    /// A student's results and statistics in one subject.
    /// </summary>
    /// <param name="SubjectName">The full name of the subject.</param>
    /// <param name="SubjectCode">The short code of the subject.</param>
    /// <param name="Average">The formatted average, or <c>-</c> if there are no counted results.</param>
    /// <param name="HasPerformanceDropWarning">Whether a performance drop was detected.</param>
    /// <param name="Assessments">The results in this subject.</param>
    public record SubjectDetailGroup(
        string SubjectName,
        string SubjectCode,
        string Average,
        bool HasPerformanceDropWarning,
        List<Assessment> Assessments
    );
}
