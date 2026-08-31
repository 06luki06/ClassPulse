using CommunityToolkit.Mvvm.ComponentModel;

namespace At.luki0606.ClassPulse.ViewModels.Dialogs
{
    public partial class InputField : ObservableObject
    {
        public string Label { get; }
        public string Placeholder { get; }

        [ObservableProperty]
        private string _value;

        public InputField(string label, string placeholder, string initialValue = "")
        {
            Label = label;
            Placeholder = placeholder;
            _value = initialValue;
        }
    }
}
