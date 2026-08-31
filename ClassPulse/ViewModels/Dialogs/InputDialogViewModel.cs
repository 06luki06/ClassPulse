using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;

namespace At.luki0606.ClassPulse.ViewModels.Dialogs
{
    public partial class InputDialogViewModel : ObservableObject
    {
        public string? Title { get; }
        public string? Message { get; }
        public List<InputField>? Fields { get; }

        public bool IsConfirmed { get; private set; }

        public event Action? OnCloseRequested;

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

        public string GetValue(string label)
        {
            InputField? field = Fields?.Find(f => f.Label == label);
            return field?.Value ?? string.Empty;
        }
    }
}
