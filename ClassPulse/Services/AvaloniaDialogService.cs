using At.luki0606.ClassPulse.ViewModels.Dialogs;
using At.luki0606.ClassPulse.Views.Dialogs;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using System.Threading.Tasks;

namespace At.luki0606.ClassPulse.Services
{
    public class AvaloniaDialogService : IDialogService
    {
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
    }
}
