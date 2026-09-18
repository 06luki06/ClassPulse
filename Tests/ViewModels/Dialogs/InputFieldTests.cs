using At.luki0606.ClassPulse.ViewModels.Dialogs;

namespace At.luki0606.ClassPulse.Tests.ViewModels.Dialogs
{
    [TestFixture]
    public class InputFieldTests
    {
        [Test]
        public void InputField_WhenSingleLine_SetsFlagsCorrectly()
        {
            InputField field = new("Label", "Placeholder", "InitialValue", isMultiline: false);

            Assert.That(field.IsMultiline, Is.False);
            Assert.That(field.IsDropdown, Is.False);
            Assert.That(field.IsSingleLine, Is.True);
        }

        [Test]
        public void InputField_WhenMultiline_SetsFlagsCorrectly()
        {
            InputField field = new("Label", "Placeholder", "InitialValue", isMultiline: true);

            Assert.That(field.IsMultiline, Is.True);
            Assert.That(field.IsDropdown, Is.False);
            Assert.That(field.IsSingleLine, Is.False);
        }

        [Test]
        public void InputField_WhenDropdown_SetsFlagsCorrectly()
        {
            InputField field = new("Label", "Placeholder", "InitialValue", options: ["Option1", "Option2"]);

            Assert.That(field.IsMultiline, Is.False);
            Assert.That(field.IsDropdown, Is.True);
            Assert.That(field.IsSingleLine, Is.False);
        }
    }
}