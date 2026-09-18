using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;

namespace At.luki0606.ClassPulse.ViewModels.Dialogs
{
    public partial class InputField : ObservableObject
    {
        public string Label { get; }
        public string Placeholder { get; }
        public List<string>? Options { get; }
        public bool IsDropdown => Options is { Count: > 0 };
        public bool IsMultiline { get; }
        public bool IsSingleLine => !IsDropdown && !IsMultiline;

        [ObservableProperty]
        private string _value;

        public InputField(string label, string placeholder, string initialValue = "", IEnumerable<string>? options = null, bool isMultiline = false)
        {
            Label = label;
            Placeholder = placeholder;
            _value = initialValue;
            Options = options != null ? [.. options] : null;
            IsMultiline = isMultiline;
        }
    }
}
