using Avalonia.Controls;
using System.Diagnostics.CodeAnalysis;

namespace At.luki0606.ClassPulse.Views.Dialogs;

/// <summary>
/// Generic input dialog. See <see cref="ViewModels.Dialogs.InputDialogViewModel"/>.
/// </summary>
[ExcludeFromCodeCoverage]
public partial class InputDialogWindow : Window
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public InputDialogWindow()
    {
        InitializeComponent();
    }
}