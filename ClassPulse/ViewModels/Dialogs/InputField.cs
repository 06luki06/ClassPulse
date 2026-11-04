using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;

namespace At.luki0606.ClassPulse.ViewModels.Dialogs
{
    /// <summary>
    /// One field of the generic input dialog: a dropdown, a multi-line box or a single-line box.
    /// </summary>
    public partial class InputField : ObservableObject
    {
        /// <summary>
        /// Gets the label, which also identifies the field in <see cref="InputDialogViewModel.GetValue"/>.
        /// </summary>
        public string Label { get; }
        /// <summary>
        /// Gets the placeholder text.
        /// </summary>
        public string Placeholder { get; }
        /// <summary>
        /// Gets the dropdown options, or <see langword="null"/> if this is a text field.
        /// </summary>
        public List<string>? Options { get; }
        /// <summary>
        /// Gets a value indicating whether the field has options to choose from.
        /// </summary>
        public bool IsDropdown => Options is { Count: > 0 };
        /// <summary>
        /// Gets a value indicating whether the field is a multi-line text box.
        /// </summary>
        public bool IsMultiline { get; }
        /// <summary>
        /// Gets a value indicating whether the field is a single-line text box.
        /// </summary>
        public bool IsSingleLine => !IsDropdown && !IsMultiline;

        /// <summary>
        /// The current value.
        /// </summary>
        [ObservableProperty]
        private string _value;

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="label">The label.</param>
        /// <param name="placeholder">The placeholder text.</param>
        /// <param name="initialValue">The initial value.</param>
        /// <param name="options">The dropdown options, or <see langword="null"/> for a text field.</param>
        /// <param name="isMultiline">Whether a text field has multiple lines.</param>
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
