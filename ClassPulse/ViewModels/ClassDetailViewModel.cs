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

                foreach (Subject subject in allSubjects)
                {
                    IEnumerable<Assessment> studentAssessments = student.GetAssessmentsBySubjectId(subject.Id);
                    double avg = _assessmentService.CalculateSubjectAverage(studentAssessments);
                    string gradeStr = avg > 0 ? avg.ToString("0.0") : "-";

                    row.SubjectGradesList.Add(new SubjectGradeDto(subject.Code, gradeStr));
                }

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
                    await _classService.AddStudentToSchoolSclass(SelectedClass.Id, firstName, lastName);
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

            // has to get replaced by subject-choice
            SubjectDto firstSubject = Subjects.First();

            InputDialogResult? result = await _dialogService.ShowInputDialogAsync(
                Resources.Resources.Dialog_NewAssessment_Title,
                string.Format(Resources.Resources.Dialog_NewAssessment_Message, firstSubject),
                new InputField(Resources.Resources.Label_Titel, $"{Resources.Resources.Label_for_example_abbr} 1. {Resources.Resources.Label_Test}"),
                new InputField(Resources.Resources.Label_Weight, "1", "1")
            );

            if (result is { IsConfirmed: true })
            {
                string title = result.ViewModel.GetValue(Resources.Resources.Label_Titel);
                string weightStr = result.ViewModel.GetValue(Resources.Resources.Label_Weight);
                if (!string.IsNullOrWhiteSpace(title) && int.TryParse(weightStr, out int weight))
                {
                    await _assessmentService.CreateClassAssessmentAsync(
                        schoolClassId: SelectedClass.Id,
                        subjectId: firstSubject.Id,
                        title: title,
                        weight: weight,
                        date: DateTime.Now,
                        defaultGrade: 1.0
                    );

                    await LoadDataAsync();
                }
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
