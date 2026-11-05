using At.luki0606.ClassPulse.ViewModels.Dialogs;
using At.luki0606.ClassPulse.Views.Dialogs;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using System.Threading.Tasks;

namespace At.luki0606.ClassPulse.Services
{
    /// <summary>
    /// <see cref="IDialogService"/> implementation that opens Avalonia windows.
    /// </summary>
    public class AvaloniaDialogService : IDialogService
    {
        /// <inheritdoc/>
        public async Task<InputDialogResult?> ShowInputDialogAsync(string title, string message, params InputField[] fields)
        {
            InputDialogViewModel dialogVm = new(title, message, fields);
            InputDialogWindow dialog = new() { DataContext = dialogVm };

            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop && desktop.MainWindow != null)
            {
                dialogVm.OnCloseRequested += () => dialog.Close();

                await dialog.ShowDialog<bool>(desktop.MainWindow);
                return new InputDialogResult(dialogVm.IsConfirmed, dialogVm);
            }

            return null;
        }

        /// <inheritdoc/>
        public async Task ShowMessageAsync(string title, string message)
        {
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop && desktop.MainWindow != null)
            {
                MessageDialogViewModel dialogVm = new(title, message);
                MessageDialogWindow dialog = new() { DataContext = dialogVm };
                dialogVm.OnCloseRequested += () => dialog.Close();

                await dialog.ShowDialog(desktop.MainWindow);
            }
        }
    }
}
