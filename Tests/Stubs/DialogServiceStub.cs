using At.luki0606.ClassPulse.Services;
using At.luki0606.ClassPulse.ViewModels.Dialogs;

namespace At.luki0606.ClassPulse.Tests.Stubs
{
    internal class DialogServiceStub : IDialogService
    {
        public InputDialogResult? NextInputResult { get; set; }

        public Task<InputDialogResult?> ShowInputDialogAsync(string title, string message, params InputField[] fields)
        {
            return Task.FromResult(NextInputResult);
        }
    }
}
