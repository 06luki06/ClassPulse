using Avalonia.Controls;
using System.Diagnostics.CodeAnalysis;

namespace At.luki0606.ClassPulse.Views;

/// <summary>
/// The application's only top-level page window. It shows the current page of <see cref="ViewModels.MainWindowViewModel"/>.
/// </summary>
[ExcludeFromCodeCoverage]
public partial class MainWindow : Window
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();
    }
}
