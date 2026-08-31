using At.luki0606.ClassPulse.ViewModels.Dialogs;
using System.Threading.Tasks;

namespace At.luki0606.ClassPulse.Services
{
    public interface IDialogService
    {
        Task<InputDialogResult?> ShowInputDialogAsync(string title, string message, params InputField[] fields);
    }

    public record InputDialogResult(bool IsConfirmed, InputDialogViewModel ViewModel);
}
