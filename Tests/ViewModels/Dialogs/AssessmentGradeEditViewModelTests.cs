using At.luki0606.ClassPulse.Data.Entities;
using At.luki0606.ClassPulse.Tests.Stubs;
using At.luki0606.ClassPulse.ViewModels.Dialogs;

namespace At.luki0606.ClassPulse.Tests.ViewModels.Dialogs
{
    [TestFixture]
    internal class AssessmentGradeEditViewModelTests
    {
        private AssessmentServiceStub _assessmentService;
        private Guid _studentId;
        private Guid _subjectId;
        private Assessment _testAssessment;

        [SetUp]
        public void Setup()
        {
            _assessmentService = new AssessmentServiceStub();
            _studentId = Guid.NewGuid();
            _subjectId = Guid.NewGuid();

            _testAssessment = new Assessment("Math Test", DateTime.Now, 1.0, _studentId, _subjectId, 1);
        }

        [Test]
        public void Constructor_ShouldPopulateGradeEntries()
        {
            List<Assessment> assessments = [_testAssessment];

            AssessmentGradeEditViewModel vm = new("Math Test", assessments, _assessmentService);

            Assert.That(vm.Title, Is.EqualTo("Math Test"));
            Assert.That(vm.GradeEntries, Has.Count.EqualTo(1));
            Assert.That(vm.GradeEntries[0].AssessmentId, Is.EqualTo(_testAssessment.Id));
            Assert.That(vm.GradeEntries[0].Grade, Is.EqualTo(1.0));
            Assert.That(vm.IsConfirmed, Is.False);
        }

        [Test]
        public async Task SaveAsync_ShouldUpdateAssessmentGradesAndConfirm()
        {
            Guid schoolClassId = Guid.NewGuid();

            _assessmentService.RegisterStudentToClass(_studentId, schoolClassId);
            await _assessmentService.CreateClassAssessmentAsync(schoolClassId, _subjectId, "Math Test", DateTime.Now);

            List<Assessment> assessments = await _assessmentService.GetAssessmentsByTitleAndSubjectAsync(schoolClassId, _subjectId, "Math Test");

            AssessmentGradeEditViewModel vm = new("Math Test", assessments, _assessmentService);

            bool closeRequested = false;
            vm.OnCloseRequested += () => closeRequested = true;

            vm.GradeEntries[0].Grade = 2.0;
            vm.GradeEntries[0].PositiveFeedback = "Good job";
            vm.GradeEntries[0].ImprovementNotes = "Work on speed";

            await vm.SaveCommand.ExecuteAsync(null);

            Assert.That(vm.IsConfirmed, Is.True);
            Assert.That(closeRequested, Is.True);

            List<Assessment> updatedAssessments = await _assessmentService.GetAssessmentsByTitleAndSubjectAsync(schoolClassId, _subjectId, "Math Test");
            Assert.That(updatedAssessments[0].Grade, Is.EqualTo(2.0));
            Assert.That(updatedAssessments[0].PositiveFeedback, Is.EqualTo("Good job"));
            Assert.That(updatedAssessments[0].ImprovementNotes, Is.EqualTo("Work on speed"));
        }

        [Test]
        public void Cancel_ShouldNotConfirmAndTriggerClose()
        {
            List<Assessment> assessments = [_testAssessment];
            AssessmentGradeEditViewModel vm = new("Math Test", assessments, _assessmentService);

            bool closeRequested = false;
            vm.OnCloseRequested += () => closeRequested = true;

            vm.CancelCommand.Execute(null);

            Assert.That(vm.IsConfirmed, Is.False);
            Assert.That(closeRequested, Is.True);
        }
    }
}
