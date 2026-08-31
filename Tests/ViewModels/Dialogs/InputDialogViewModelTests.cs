using At.luki0606.ClassPulse.ViewModels.Dialogs;

namespace At.luki0606.ClassPulse.Tests.ViewModels.Dialogs
{
    [TestFixture]
    public class InputDialogViewModelTests
    {
        private InputDialogViewModel _vm;

        [SetUp]
        public void SetUp()
        {
            _vm = new("Title", "Message", []);
        }

        [Test]
        public void Cancel_ReturnsFalse()
        {
            _vm.CancelCommand.Execute(null);
            Assert.That(_vm.IsConfirmed, Is.False);
        }

        [Test]
        public void Confirm_ReturnsTrue()
        {
            _vm.ConfirmCommand.Execute(null);
            Assert.That(_vm.IsConfirmed, Is.True);
        }

        [Test]
        public void GetValue_NotFound_ReturnsEmptyString()
        {
            string result = _vm.GetValue("NotPresent");
            Assert.That(result, Is.EqualTo(string.Empty));
        }

        [Test]
        public void GetValue_Present_ReturnsValue()
        {
            _vm = new("Title", "Message", [new InputField("Present", "placeholder", "InitialValue")]);
            string result = _vm.GetValue("Present");
            Assert.That(result, Is.EqualTo("InitialValue"));
        }
    }
}
