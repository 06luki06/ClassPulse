using At.luki0606.ClassPulse.ViewModels.Dialogs;
using System.Threading.Tasks;

namespace At.luki0606.ClassPulse.Services
{
    /// <summary>
    /// Shows dialogs from view models without a dependency on view types.
    /// </summary>
    public interface IDialogService
    {
        /// <summary>
        /// Shows a modal input dialog and waits until it closes.
        /// </summary>
        /// <param name="title">The window title.</param>
        /// <param name="message">The message above the fields.</param>
        /// <param name="fields">The input fields to show.</param>
        /// <returns>The result, or <see langword="null"/> if no main window is available.</returns>
        Task<InputDialogResult?> ShowInputDialogAsync(string title, string message, params InputField[] fields);
    }

    /// <summary>
    /// The outcome of an input dialog.
    /// </summary>
    /// <param name="IsConfirmed">Whether the user confirmed the dialog.</param>
    /// <param name="ViewModel">The dialog's view model, which holds the entered values.</param>
    public record InputDialogResult(bool IsConfirmed, InputDialogViewModel ViewModel);
}
