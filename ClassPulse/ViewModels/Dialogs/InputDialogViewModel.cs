using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;

namespace At.luki0606.ClassPulse.ViewModels.Dialogs
{
    /// <summary>
    /// View model of the generic input dialog.
    /// </summary>
    public partial class InputDialogViewModel : ObservableObject
    {
        /// <summary>
        /// Gets the window title.
        /// </summary>
        public string? Title { get; }
        /// <summary>
        /// Gets the message above the fields.
        /// </summary>
        public string? Message { get; }
        /// <summary>
        /// Gets the input fields.
        /// </summary>
        public List<InputField>? Fields { get; }

        /// <summary>
        /// Gets a value indicating whether the user confirmed the dialog.
        /// </summary>
        public bool IsConfirmed { get; private set; }

        /// <summary>
        /// Occurs when the dialog should close.
        /// </summary>
        public event Action? OnCloseRequested;

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="title">The window title.</param>
        /// <param name="message">The message above the fields.</param>
        /// <param name="fields">The input fields.</param>
        public InputDialogViewModel(string title, string message, IEnumerable<InputField> fields)
        {
            Title = title;
            Message = message;
            Fields = [.. fields];
        }

        [RelayCommand]
        private void Cancel()
        {
            IsConfirmed = false;
            OnCloseRequested?.Invoke();
        }

        [RelayCommand]
        private void Confirm()
        {
            IsConfirmed = true;
            OnCloseRequested?.Invoke();
        }

        /// <summary>
        /// Gets the value of the first field with the given label.
        /// </summary>
        /// <param name="label">The label of the field.</param>
        /// <returns>The value, or an empty string if no field has this label.</returns>
        public string GetValue(string label)
        {
            InputField? field = Fields?.Find(f => f.Label == label);
            return field?.Value ?? string.Empty;
        }
    }
}
