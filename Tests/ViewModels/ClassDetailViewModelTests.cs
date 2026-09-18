using At.luki0606.ClassPulse.Data.Entities;
using At.luki0606.ClassPulse.Services;
using At.luki0606.ClassPulse.Tests.Stubs;
using At.luki0606.ClassPulse.ViewModels;
using At.luki0606.ClassPulse.ViewModels.Dialogs;

namespace At.luki0606.ClassPulse.Tests.ViewModels
{
    [TestFixture]
    internal class ClassDetailViewModelTests
    {
        private ClassServiceStub _classService;
        private AssessmentServiceStub _assessmentService;
        private DialogServiceStub _dialogService;
        private SchoolClass _testClass;

        [SetUp]
        public async Task Setup()
        {
            _classService = new ClassServiceStub();
            _assessmentService = new AssessmentServiceStub();
            _dialogService = new DialogServiceStub();

            _testClass = await _classService.CreateClassAsync("1A", "2025/2026");
        }

        [Test]
        public void ClassTitle_ShouldReturnFormattedString()
        {
            ClassDetailViewModel vm = new(_testClass, _classService, _dialogService, _assessmentService);

            Assert.That(vm.ClassTitle, Is.EqualTo("1A / 2025/2026"));
        }

        [Test]
        public async Task LoadDataAsync_ShouldPopulateSubjectsAndStudentRows()
        {
            await _classService.CreateSubjectAsync("Mathematics", "M");
            await _classService.AddStudentToSchoolClassAsync(_testClass.Id, "Max", "Mustermann");

            ClassDetailViewModel vm = new(_testClass, _classService, _dialogService, _assessmentService);

            // Wait briefly for the async constructor fire-and-forget task to complete
            await Task.Delay(50);

            Assert.That(vm.Subjects, Is.Not.Empty);
            Assert.That(vm.StudentRows, Is.Not.Empty);
            Assert.That(vm.StudentRows[0].FullName, Is.EqualTo("Max Mustermann"));
            Assert.That(vm.StudentRows[0].SubjectGradesList, Is.Not.Empty);
            Assert.That(vm.StudentRows[0].SubjectGradesList[0].ShortName, Is.EqualTo("M"));
        }

        [Test]
        public async Task AddSubjectAsync_WhenConfirmed_ShouldCreateSubjectAndReload()
        {
            InputDialogViewModel dialogResultVm = new(
                "TestTitle",
                "TestMessage",
                [
                    new InputField(Resources.Resources.Label_SubjectName, "Physics", "Physics"),
                    new InputField(Resources.Resources.Label_ShortName, "PH", "PH")
                ]
            );

            _dialogService.NextInputResult = new InputDialogResult(true, dialogResultVm);

            ClassDetailViewModel vm = new(_testClass, _classService, _dialogService, _assessmentService);

            await vm.AddSubjectCommand.ExecuteAsync(null);

            List<Subject> subjects = await _classService.GetAllSubjectsAsync();
            Assert.That(subjects, Has.Count.EqualTo(1));
            Assert.That(subjects[0].Name, Is.EqualTo("Physics"));
            Assert.That(subjects[0].Code, Is.EqualTo("PH"));
            Assert.That(vm.Subjects, Has.Count.EqualTo(1));
        }

        [Test]
        public async Task AddStudentAsync_WhenConfirmed_ShouldAddStudentAndReload()
        {
            InputDialogViewModel dialogResultVm = new(
                "TestTitle",
                "TestMessage",
                [
                    new InputField(Resources.Resources.Label_FirstName, "Erika", "Erika"),
                    new InputField(Resources.Resources.Label_LastName, "Mustermann", "Mustermann")
                ]
            );

            _dialogService.NextInputResult = new InputDialogResult(true, dialogResultVm);

            ClassDetailViewModel vm = new(_testClass, _classService, _dialogService, _assessmentService);

            await vm.AddStudentCommand.ExecuteAsync(null);

            List<Student> students = await _classService.GetStudentsByClassIdAsync(_testClass.Id);
            Assert.That(students, Has.Count.EqualTo(1));
            Assert.That(students[0].FirstName, Is.EqualTo("Erika"));
            Assert.That(vm.StudentRows, Has.Count.EqualTo(1));
        }

        [Test]
        public async Task AddAssessmentAsync_WhenConfirmed_ShouldCreateAssessmentAndReload()
        {
            Subject subject = await _classService.CreateSubjectAsync("Mathematics", "M");
            Student student = await _classService.AddStudentToSchoolClassAsync(_testClass.Id, "Max", "Mustermann");

            _assessmentService.RegisterStudentToClass(student.Id, _testClass.Id);

            ClassDetailViewModel vm = new(_testClass, _classService, _dialogService, _assessmentService);
            await Task.Delay(50);

            List<InputField> fields =
            [
                new InputField(Resources.Resources.Label_Subject, "Placeholder", "M - Mathematics"),
                new InputField(Resources.Resources.Label_Titel, "Placeholder", "Test 1"),
                new InputField(Resources.Resources.Label_Weight, "1", "2")
            ];
            InputDialogViewModel dialogResultVm = new("TestTitle", "TestMessage", fields);
            _dialogService.NextInputResult = new InputDialogResult(true, dialogResultVm);

            await vm.AddAssessmentCommand.ExecuteAsync(null);

            List<Assessment> assessments = await _assessmentService.GetAssessmentsByTitleAndSubjectAsync(_testClass.Id, subject.Id, "Test 1");
            Assert.That(assessments, Is.Not.Empty);
            Assert.That(assessments[0].Title, Is.EqualTo("Test 1"));
            Assert.That(assessments[0].Weight, Is.EqualTo(2));
        }

        [Test]
        public async Task RemoveStudentCommand_RemovesStudent()
        {
            ClassDetailViewModel vm = new(_testClass, _classService, _dialogService, _assessmentService);
            await _classService.AddStudentToSchoolClassAsync(_testClass.Id, "Test", "User");
            await vm.LoadDataAsync();

            Assert.That(vm.StudentRows, Has.Count.EqualTo(1));

            StudentMatrixRow student = vm.StudentRows[0];

            await vm.RemoveStudentCommand.ExecuteAsync(student);
            Assert.That(vm.StudentRows, Has.Count.EqualTo(0));
        }

        [Test]
        public async Task EditStudentNotes_WhenConfirmed_ShouldUpdateNotesAndReload()
        {
            Student student = await _classService.AddStudentToSchoolClassAsync(_testClass.Id, "Max", "Mustermann", "Alte Notiz");
            ClassDetailViewModel vm = new(_testClass, _classService, _dialogService, _assessmentService);
            await vm.LoadDataAsync();

            StudentMatrixRow row = vm.StudentRows.First(r => r.Id == student.Id);

            InputDialogViewModel dialogResultVm = new(
                "Title",
                "Message",
                [
                    new InputField(Resources.Resources.Label_Notes, "Placeholder", "Neue Notiz", isMultiline: true)
                ]
            );

            _dialogService.NextInputResult = new InputDialogResult(true, dialogResultVm);

            await vm.EditStudentNotesCommand.ExecuteAsync(row);

            Student? updatedStudent = await _classService.GetStudentDetailsAsync(student.Id);
            Assert.That(updatedStudent, Is.Not.Null);
            Assert.That(updatedStudent!.GeneralNotes, Is.EqualTo("Neue Notiz"));
            Assert.That(vm.StudentRows.First(r => r.Id == student.Id).GeneralNotes, Is.EqualTo("Neue Notiz"));
        }
    }
}
