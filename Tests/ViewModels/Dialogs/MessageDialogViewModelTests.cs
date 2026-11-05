using At.luki0606.ClassPulse.ViewModels.Dialogs;

namespace At.luki0606.ClassPulse.Tests.ViewModels.Dialogs
{
    [TestFixture]
    internal sealed class MessageDialogViewModelTests
    {
        [Test]
        public void Constructor_SetsTitleAndMessage()
        {
            MessageDialogViewModel vm = new("Title", "Message");

            Assert.That(vm.Title, Is.EqualTo("Title"));
            Assert.That(vm.Message, Is.EqualTo("Message"));
        }

        [Test]
        public void CloseCommand_RaisesOnCloseRequested()
        {
            MessageDialogViewModel vm = new("Title", "Message");
            int closeRequests = 0;
            vm.OnCloseRequested += () => closeRequests++;

            vm.CloseCommand.Execute(null);

            Assert.That(closeRequests, Is.EqualTo(1));
        }
    }
}
