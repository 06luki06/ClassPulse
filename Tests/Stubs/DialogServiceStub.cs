using At.luki0606.ClassPulse.Services;
using At.luki0606.ClassPulse.ViewModels.Dialogs;

namespace At.luki0606.ClassPulse.Tests.Stubs
{
    internal sealed class DialogServiceStub : IDialogService
    {
        public InputDialogResult? NextInputResult { get; set; }

        public List<(string Title, string Message)> ShownMessages { get; } = [];

        public Task<InputDialogResult?> ShowInputDialogAsync(string title, string message, params InputField[] fields)
        {
            return Task.FromResult(NextInputResult);
        }

        public Task ShowMessageAsync(string title, string message)
        {
            ShownMessages.Add((title, message));
            return Task.CompletedTask;
        }
    }
}
