using At.luki0606.ClassPulse.Data.Entities;
using At.luki0606.ClassPulse.Tests.Stubs;
using At.luki0606.ClassPulse.ViewModels;
using System.Reflection;

namespace At.luki0606.ClassPulse.Tests.ViewModels
{
    [TestFixture]
    internal class StudentDetailViewModelTests
    {
        private ClassServiceStub _classService;
        private AssessmentServiceStub _assessmentService;
        private ClassDetailViewModel _dummyParentVm;
        private DialogServiceStub _dialogServiceStub;
        private SchoolClass _testClass;
        private Student _testStudent;
        private Subject _testSubject;

        [SetUp]
        public async Task Setup()
        {
            _classService = new ClassServiceStub();
            _assessmentService = new AssessmentServiceStub();

            _testClass = await _classService.CreateClassAsync("1A", "2025/2026");
            _dialogServiceStub = new DialogServiceStub();
            _dummyParentVm = new ClassDetailViewModel(_testClass, _classService, _dialogServiceStub, _assessmentService);

            _testSubject = await _classService.CreateSubjectAsync("Mathematics", "M");
            _testStudent = await _classService.AddStudentToSchoolClassAsync(_testClass.Id, "Max", "Mustermann");

            _assessmentService.RegisterStudentToClass(_testStudent.Id, _testClass.Id);
        }

        [Test]
        public async Task Constructor_ShouldLoadStudentDataAndPopulateSubjectGroups()
        {
            Assessment assessment = new(
                    title: "Test 1",
                    date: DateTime.Now,
                    grade: 2.0,
                    studentId: _testStudent.Id,
                    subjectId: _testSubject.Id,
                    weight: 1
                );

            FieldInfo? field = typeof(Student).GetField("_assessments", BindingFlags.NonPublic | BindingFlags.Instance);
            List<Assessment> list = (List<Assessment>)field!.GetValue(_testStudent)!;
            list.Add(assessment);

            StudentDetailViewModel vm = new(_testStudent.Id, _dummyParentVm, _classService, _assessmentService);
            await Task.Delay(50);

            Assert.That(vm.StudentId, Is.EqualTo(_testStudent.Id));
            Assert.That(vm.StudentName, Is.EqualTo("Max Mustermann"));
            Assert.That(vm.CurrentStudent, Is.Not.Null);
            Assert.That(vm.SubjectGroups, Is.Not.Empty);
            Assert.That(vm.SubjectGroups[0].SubjectName, Is.EqualTo("Mathematics"));
            Assert.That(vm.SubjectGroups[0].SubjectCode, Is.EqualTo("M"));
            string expectedAverage = 2.0.ToString("0.0", System.Globalization.CultureInfo.CurrentCulture);
            Assert.That(vm.SubjectGroups[0].Average, Is.EqualTo(expectedAverage));
        }

        [Test]
        public async Task LoadStudentData_WhenStudentNotFound_ShouldHandleGracefully()
        {
            Guid nonExistentId = Guid.NewGuid();

            StudentDetailViewModel vm = new(nonExistentId, _dummyParentVm, _classService, _assessmentService);
            await Task.Delay(50);

            Assert.That(vm.CurrentStudent, Is.Null);
            Assert.That(vm.StudentName, Is.EqualTo(string.Empty));
            Assert.That(vm.SubjectGroups, Is.Empty);
        }

        [Test]
        public async Task SubjectGroups_ShouldReflectPerformanceDropWarning()
        {
            List<Assessment> assessments =
            [
                new("T1", DateTime.Now.AddDays(-3), 1.0, _testStudent.Id, _testSubject.Id, 1),
                new("T2", DateTime.Now.AddDays(-2), 1.0, _testStudent.Id, _testSubject.Id, 1),
                new("T3", DateTime.Now.AddDays(-1), 5.0, _testStudent.Id, _testSubject.Id, 1) // Performance Drop
            ];

            FieldInfo? field = typeof(Student).GetField("_assessments", BindingFlags.NonPublic | BindingFlags.Instance);
            List<Assessment> list = (List<Assessment>)field!.GetValue(_testStudent)!;
            list.AddRange(assessments);

            StudentDetailViewModel vm = new(_testStudent.Id, _dummyParentVm, _classService, _assessmentService);
            await Task.Delay(50);

            Assert.That(vm.SubjectGroups, Is.Not.Empty);
            Assert.That(vm.SubjectGroups[0].Assessments, Has.Count.EqualTo(3));
        }
    }
}
