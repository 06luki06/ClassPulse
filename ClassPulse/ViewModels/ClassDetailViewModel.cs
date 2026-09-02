using At.luki0606.ClassPulse.Data.Entities;
using At.luki0606.ClassPulse.Services;
using At.luki0606.ClassPulse.ViewModels.Dialogs;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace At.luki0606.ClassPulse.ViewModels
{
    public partial class ClassDetailViewModel : ViewModelBase
    {
        private readonly IClassService _classService;
        private readonly IDialogService _dialogService;
        private readonly IAssessmentService _assessmentService;

        public SchoolClass SelectedClass { get; }
        public string ClassTitle => $"{SelectedClass.Name} / {SelectedClass.SchoolYear}";

        public ObservableCollection<StudentMatrixRow> StudentRows { get; } = [];
        public ObservableCollection<SubjectDto> Subjects { get; } = [];

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
                    string gradeStr = avg > 0 ? avg.ToString("0.0") : "-";

                    row.SubjectGradesList.Add(new SubjectGradeDto(subject.Code, gradeStr));
                }
                double overallAvg = _assessmentService.CalculateSubjectAverage(allStudentAssessments);
                row.OverallAverage = overallAvg > 0 ? overallAvg.ToString("0.0") : "-";
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
                    await _classService.AddStudentToSchoolClass(SelectedClass.Id, firstName, lastName);
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

                if (!string.IsNullOrWhiteSpace(title) && int.TryParse(weightStr, out int weight))
                {
                    await _assessmentService.CreateClassAssessmentAsync(
                        schoolClassId: SelectedClass.Id,
                        subjectId: selectedSubject.Id,
                        title: title,
                        weight: weight,
                        date: DateTime.Now,
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
    }

    public class SubjectDto
    {
        private readonly Subject _subject;

        public Guid Id => _subject.Id;
        public string Name => _subject.Name;
        public string ShortName => _subject.Code;

        public SubjectDto(Subject subject)
        {
            _subject = subject;
        }
    }

    public class StudentMatrixRow
    {
        private readonly Student _student;

        public Guid Id => _student.Id;
        public string FullName => _student.FullName;
        public string OverallAverage { get; set; } = "-";
        public List<SubjectGradeDto> SubjectGradesList { get; } = [];

        public StudentMatrixRow(Student student)
        {
            _student = student;
        }
    }

    public class SubjectGradeDto
    {
        public string ShortName { get; }
        public string Grade { get; }

        public SubjectGradeDto(string shortName, string grade)
        {
            ShortName = shortName;
            Grade = grade;
        }
    }
}
