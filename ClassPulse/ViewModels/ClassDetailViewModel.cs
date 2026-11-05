using At.luki0606.ClassPulse.Data.Entities;
using At.luki0606.ClassPulse.Services;
using At.luki0606.ClassPulse.ViewModels.Dialogs;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace At.luki0606.ClassPulse.ViewModels
{
    /// <summary>
    /// View model of the class page, which shows the grade matrix and adds students, assessments and subjects.
    /// </summary>
    public partial class ClassDetailViewModel : ViewModelBase
    {
        private readonly IClassService _classService;
        private readonly IDialogService _dialogService;
        private readonly IAssessmentService _assessmentService;

        /// <summary>
        /// Gets the class shown on this page.
        /// </summary>
        public SchoolClass SelectedClass { get; }
        /// <summary>
        /// Gets the page title made of the class name and school year.
        /// </summary>
        public string ClassTitle => $"{SelectedClass.Name} / {SelectedClass.SchoolYear}";

        /// <summary>
        /// Gets one matrix row per student.
        /// </summary>
        public ObservableCollection<StudentMatrixRow> StudentRows { get; } = [];
        /// <summary>
        /// Gets the subjects, which form the matrix columns.
        /// </summary>
        public ObservableCollection<SubjectDto> Subjects { get; } = [];

        /// <summary>
        /// Initializes a new instance and starts loading the data in the background.
        /// </summary>
        /// <param name="selectedClass">The class to show.</param>
        /// <param name="classService">The class service.</param>
        /// <param name="dialogService">The dialog service.</param>
        /// <param name="assessmentService">The assessment service.</param>
        public ClassDetailViewModel(
            SchoolClass selectedClass,
            IClassService classService,
            IDialogService dialogService,
            IAssessmentService assessmentService
            )
        {
            SelectedClass = selectedClass;
            _classService = classService;
            _dialogService = dialogService;
            _assessmentService = assessmentService;

            _ = LoadDataAsync();
        }

        /// <summary>
        /// Reloads the subjects and students and recalculates all averages.
        /// </summary>
        /// <returns>A task that completes when loading has finished.</returns>
        [RelayCommand]
        public async Task LoadDataAsync()
        {
            StudentRows.Clear();
            Subjects.Clear();

            List<Subject> allSubjects = await _classService.GetAllSubjectsAsync();
            foreach (Subject sub in allSubjects)
            {
                Subjects.Add(new SubjectDto(sub));
            }

            List<Student> students = await _classService.GetStudentsByClassIdAsync(SelectedClass.Id);

            foreach (Student student in students)
            {
                StudentMatrixRow row = new(student);
                List<Assessment> allStudentAssessments = [];

                foreach (Subject subject in allSubjects)
                {
                    IEnumerable<Assessment> studentAssessments = student.GetAssessmentsBySubjectId(subject.Id);
                    allStudentAssessments.AddRange(studentAssessments);
                    double avg = _assessmentService.CalculateSubjectAverage(studentAssessments);
                    string gradeStr = avg > 0 ? avg.ToString("0.0", CultureInfo.CurrentCulture) : "-";

                    row.SubjectGradesList.Add(new SubjectGradeDto(subject.Code, gradeStr));
                }
                double overallAvg = _assessmentService.CalculateSubjectAverage(allStudentAssessments);
                row.OverallAverage = overallAvg > 0 ? overallAvg.ToString("0.0", CultureInfo.CurrentCulture) : "-";
                StudentRows.Add(row);
            }

            OnPropertyChanged(nameof(Subjects));
        }

        [RelayCommand]
        private static void BackToHome()
        {
            if (App.Current is App { Services: { } services })
            {
                MainWindowViewModel mainVm = services.GetRequiredService<MainWindowViewModel>();
                mainVm.NavigateToHome();
            }
        }

        [RelayCommand]
        private async Task AddStudentAsync()
        {
            InputDialogResult? result = await _dialogService.ShowInputDialogAsync(
                Resources.Resources.Dialog_NewPupil_Title,
                Resources.Resources.Dialog_NewPupil_Message,
                    new InputField(Resources.Resources.Label_FirstName, "Max"),
                    new InputField(Resources.Resources.Label_LastName, "Mustermann")
            );

            if (result is { IsConfirmed: true })
            {
                string firstName = result.ViewModel.GetValue(Resources.Resources.Label_FirstName);
                string lastName = result.ViewModel.GetValue(Resources.Resources.Label_LastName);
                if (!string.IsNullOrWhiteSpace(firstName) && !string.IsNullOrWhiteSpace(lastName))
                {
                    await _classService.AddStudentToSchoolClassAsync(SelectedClass.Id, firstName, lastName);
                    await LoadDataAsync();
                }
            }
        }

        [RelayCommand]
        private async Task AddAssessmentAsync()
        {
            if (!Subjects.Any())
            {
                return;
            }

            List<string> subjectOptions = [.. Subjects.Select(s => $"{s.ShortName} - {s.Name}")];

            InputField subjectField = new(
                Resources.Resources.Label_Subject,
                Resources.Resources.Dialog_SelectSubject_Message,
                subjectOptions[0],
                subjectOptions
            );

            InputField titleField = new(
                Resources.Resources.Label_Titel,
                $"{Resources.Resources.Label_for_example_abbr} 1. {Resources.Resources.Label_Test}");

            InputField weightField = new(
                Resources.Resources.Label_Weight,
                "1",
                "1"
            );

            InputDialogResult? result = await _dialogService.ShowInputDialogAsync(
                Resources.Resources.Dialog_NewAssessment_Title,
                Resources.Resources.Dialog_NewAssessment_Message,
                subjectField,
                titleField,
                weightField
            );

            if (result is { IsConfirmed: true })
            {
                string selectedOption = result.ViewModel.GetValue(Resources.Resources.Label_Subject);
                string title = result.ViewModel.GetValue(Resources.Resources.Label_Titel);
                string weightStr = result.ViewModel.GetValue(Resources.Resources.Label_Weight);

                string shortName = selectedOption.Split(" - ")[0];
                SubjectDto? selectedSubject = Subjects.FirstOrDefault(s => s.ShortName == shortName) ?? Subjects[0];

                // NumberStyles.Float excludes AllowThousands. Otherwise "0.5" under German culture would parse as 5.
                if (!double.TryParse(weightStr, NumberStyles.Float, CultureInfo.CurrentCulture, out double weight)
                    || !Assessment.IsValidWeight(weight))
                {
                    await _dialogService.ShowMessageAsync(
                        Resources.Resources.Dialog_InvalidInput_Title,
                        Resources.Resources.Dialog_InvalidWeight_Message);
                    return;
                }

                if (!string.IsNullOrWhiteSpace(title))
                {
                    await _assessmentService.CreateClassAssessmentAsync(
                        schoolClassId: SelectedClass.Id,
                        subjectId: selectedSubject.Id,
                        title: title,
                        weight: weight,
                        assessmentDate: DateTime.Now,
                        defaultGrade: 1.0
                    );

                    await LoadDataAsync();
                }
            }
        }

        [RelayCommand]
        private async Task AddSubjectAsync()
        {
            InputDialogResult? result = await _dialogService.ShowInputDialogAsync(
                Resources.Resources.Dialog_NewSubject_Title,
                Resources.Resources.Dialog_NewSubject_Message,
                new InputField(Resources.Resources.Label_SubjectName, Resources.Resources.Label_Mathematics),
                new InputField(Resources.Resources.Label_ShortName, "M")
            );
            if (result is { IsConfirmed: true })
            {
                string name = result.ViewModel.GetValue(Resources.Resources.Label_SubjectName);
                string code = result.ViewModel.GetValue(Resources.Resources.Label_ShortName);
                if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(code))
                {
                    await _classService.CreateSubjectAsync(name, code);
                    await LoadDataAsync();
                }
            }
        }

        [RelayCommand]
        private void SelectStudent(StudentMatrixRow row)
        {
            if (App.Current is App { Services: { } services })
            {
                MainWindowViewModel mainVm = services.GetRequiredService<MainWindowViewModel>();
                StudentDetailViewModel studentDetailVm = new(row.Id, this, _classService, _assessmentService);
                mainVm.NavigateToStudentDetail(studentDetailVm);
            }
        }

        [RelayCommand]
        private async Task RemoveStudent(StudentMatrixRow row)
        {
            await _classService.RemoveStudentFromClassAsync(SelectedClass.Id, row.Id);
            await LoadDataAsync();
        }

        [RelayCommand]
        private async Task EditStudentNotes(StudentMatrixRow row)
        {
            InputDialogResult? result = await _dialogService.ShowInputDialogAsync(
                title: $"{Resources.Resources.Label_Notes}: {row.FullName}",
                message: Resources.Resources.Label_Notes,
                new InputField(
                    label: Resources.Resources.Label_Notes,
                    placeholder: Resources.Resources.Label_Notes,
                    initialValue: row.GeneralNotes,
                    isMultiline: true
                )
            );

            if (result is { IsConfirmed: true })
            {
                string notes = result.ViewModel.GetValue(Resources.Resources.Label_Notes);
                await _classService.UpdateStudentGeneralNotesAsync(row.Id, notes);
                await LoadDataAsync();
            }
        }
    }

    /// <summary>
    /// Display data of a subject.
    /// </summary>
    public class SubjectDto
    {
        private readonly Subject _subject;

        /// <summary>
        /// Gets the identifier of the subject.
        /// </summary>
        public Guid Id => _subject.Id;
        /// <summary>
        /// Gets the full name of the subject.
        /// </summary>
        public string Name => _subject.Name;
        /// <summary>
        /// Gets the short code of the subject.
        /// </summary>
        public string ShortName => _subject.Code;

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="subject">The subject to display.</param>
        public SubjectDto(Subject subject)
        {
            _subject = subject;
        }
    }

    /// <summary>
    /// One student's row in the grade matrix.
    /// </summary>
    public class StudentMatrixRow
    {
        private readonly Student _student;

        /// <summary>
        /// Gets the identifier of the student.
        /// </summary>
        public Guid Id => _student.Id;
        /// <summary>
        /// Gets the student's full name.
        /// </summary>
        public string FullName => _student.FullName;
        /// <summary>
        /// Gets the general notes, or an empty string if there are none.
        /// </summary>
        public string GeneralNotes => _student.GeneralNotes ?? string.Empty;
        /// <summary>
        /// Gets or sets the formatted average across all subjects, or <c>-</c> if there are no counted results.
        /// </summary>
        public string OverallAverage { get; set; } = "-";
        /// <summary>
        /// Gets the formatted average per subject, in column order.
        /// </summary>
        public List<SubjectGradeDto> SubjectGradesList { get; } = [];

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="student">The student to display.</param>
        public StudentMatrixRow(Student student)
        {
            _student = student;
        }
    }

    /// <summary>
    /// One cell of the grade matrix.
    /// </summary>
    public class SubjectGradeDto
    {
        /// <summary>
        /// Gets the short code of the subject.
        /// </summary>
        public string ShortName { get; }
        /// <summary>
        /// Gets the formatted average, or <c>-</c> if there are no counted results.
        /// </summary>
        public string Grade { get; }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="shortName">The short code of the subject.</param>
        /// <param name="grade">The formatted average.</param>
        public SubjectGradeDto(string shortName, string grade)
        {
            ShortName = shortName;
            Grade = grade;
        }
    }
}
