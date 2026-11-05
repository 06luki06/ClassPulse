using CommunityToolkit.Mvvm.Input;
using System;

namespace At.luki0606.ClassPulse.ViewModels.Dialogs
{
    /// <summary>
    /// View model of the message dialog, which shows text and can only be closed.
    /// </summary>
    public partial class MessageDialogViewModel
    {
        /// <summary>
        /// Gets the window title.
        /// </summary>
        public string Title { get; }
        /// <summary>
        /// Gets the message to show.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Occurs when the dialog should close.
        /// </summary>
        public event Action? OnCloseRequested;

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="title">The window title.</param>
        /// <param name="message">The message to show.</param>
        public MessageDialogViewModel(string title, string message)
        {
            Title = title;
            Message = message;
        }

        [RelayCommand]
        private void Close()
        {
            OnCloseRequested?.Invoke();
        }
    }
}
