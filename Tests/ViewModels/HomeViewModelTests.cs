using At.luki0606.ClassPulse.Data.Entities;
using At.luki0606.ClassPulse.Services;
using At.luki0606.ClassPulse.Tests.Stubs;
using At.luki0606.ClassPulse.ViewModels;
using At.luki0606.ClassPulse.ViewModels.Dialogs;

namespace At.luki0606.ClassPulse.Tests.ViewModels
{
    [TestFixture]
    public class HomeViewModelTests
    {
        private ClassServiceStub _classServiceStub = null!;
        private DialogServiceStub _dialogServiceStub = null!;
        private HomeViewModel _viewModel = null!;

        [SetUp]
        public void SetUp()
        {
            _classServiceStub = new ClassServiceStub();
            _dialogServiceStub = new DialogServiceStub();

            _viewModel = new HomeViewModel(_classServiceStub, _dialogServiceStub);
        }

        [Test]
        public async Task LoadClassesAsync_ShouldPopulateSchoolClassesCollection()
        {
            await _classServiceStub.CreateClassAsync("1A", "2025/2026");
            await _classServiceStub.CreateClassAsync("2B", "2025/2026");

            await _viewModel.LoadClassesAsync();

            Assert.That(_viewModel.SchoolClasses, Has.Count.EqualTo(2));
            Assert.That(_viewModel.SchoolClasses[0].Name, Is.EqualTo("1A"));
            Assert.That(_viewModel.SchoolClasses[1].Name, Is.EqualTo("2B"));
        }

        [Test]
        public async Task CreateClassAsync_WhenDialogConfirmed_ShouldAddClassToServiceAndCollection()
        {
            _dialogServiceStub.NextInputResult = new InputDialogResult(
                IsConfirmed: true,
                ViewModel: new InputDialogViewModel("Title", "Message",
                [
                    new InputField(Resources.Resources.Label_ClassName, "", "3C"),
                    new InputField(Resources.Resources.Label_SchoolYear, "", "2026/2027")
                ])
            );

            Assert.That(_viewModel.SelectedClass, Is.Null);

            await _viewModel.CreateClassCommand.ExecuteAsync(null);

            Assert.That(_viewModel.SchoolClasses, Has.Count.EqualTo(1));
            Assert.That(_viewModel.SchoolClasses[0].Name, Is.EqualTo("3C"));

            List<SchoolClass> classesInService = await _classServiceStub.GetAllClassesAsync();
            Assert.That(classesInService, Has.Count.EqualTo(1));
            Assert.That(_viewModel.SelectedClass, Is.Not.Null);
        }

        [Test]
        public async Task CreateClassAsync_WhenDialogCanceled_ShouldNotAddClass()
        {
            _dialogServiceStub.NextInputResult = new InputDialogResult(
                IsConfirmed: false,
                ViewModel: new InputDialogViewModel("3C", "2025/2026", [])
            );

            Assert.That(_viewModel.SelectedClass, Is.Null);

            await _viewModel.CreateClassCommand.ExecuteAsync(null);

            Assert.That(_viewModel.SchoolClasses, Is.Empty);
            Assert.That(_viewModel.SelectedClass, Is.Null);
        }

        [Test]
        public async Task DeleteClassAsync_ShouldRemoveSelectedClass()
        {
            SchoolClass createdClass = await _classServiceStub.CreateClassAsync("1A", "2025/2026");
            await _viewModel.LoadClassesAsync();
            _viewModel.SelectedClass = createdClass;

            await _viewModel.DeleteClassCommand.ExecuteAsync(null);

            Assert.That(_viewModel.SchoolClasses, Is.Empty);
            Assert.That(_viewModel.SelectedClass, Is.Null);

            List<SchoolClass> classesInService = await _classServiceStub.GetAllClassesAsync();
            Assert.That(classesInService, Is.Empty);
        }

        [Test]
        public async Task DeleteClassCommand_CanExecute_ShouldDependOnSelectedClass()
        {
            SchoolClass createdClass = await _classServiceStub.CreateClassAsync("1A", "2025/2026");
            await _viewModel.LoadClassesAsync();

            _viewModel.SelectedClass = null;

            Assert.That(_viewModel.DeleteClassCommand.CanExecute(null), Is.False);

            _viewModel.SelectedClass = createdClass;

            Assert.That(_viewModel.DeleteClassCommand.CanExecute(null), Is.True);
        }

        [Test]
        public async Task Ctor_SelectFirstEntry_WhenItemsArePresent()
        {
            SchoolClass createdClass = await _classServiceStub.CreateClassAsync("1A", "2025/2026");
            _viewModel = new(_classServiceStub, _dialogServiceStub);
            Assert.That(_viewModel.SelectedClass, Is.EqualTo(createdClass));
        }

        [Test]
        public async Task DeleteClassCommand_HasStillASelectedClass_AfterDeletion()
        {
            SchoolClass createdClass1 = await _classServiceStub.CreateClassAsync("1A", "2026/2027");
            SchoolClass createdClass2 = await _classServiceStub.CreateClassAsync("2A", "2026/2027");

            await _viewModel.LoadClassesAsync();

            Assert.That(_viewModel.SelectedClass, Is.EqualTo(createdClass1));
            Assert.That(_viewModel.SchoolClasses, Has.Count.EqualTo(2));

            await _viewModel.DeleteClassCommand.ExecuteAsync(null);

            Assert.That(_viewModel.SchoolClasses, Has.Count.EqualTo(1));
            Assert.That(_viewModel.SelectedClass, Is.EqualTo(createdClass2));
        }
    }
}
