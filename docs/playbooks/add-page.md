# Add a page

Example page name: `Report`.

1. Create `ClassPulse/ViewModels/ReportViewModel.cs`, deriving from `ViewModelBase`. Make the class `partial`.
2. Create `ClassPulse/Views/ReportView.axaml` and `ReportView.axaml.cs`.
   - The names must match the view model, because `ViewLocator` replaces `ViewModel` with `View`.
   - Set `x:DataType` to the view model.
   - Code-behind contains only `InitializeComponent()`, plus `[ExcludeFromCodeCoverage]` like the other views.
3. Add a `NavigateToReport` method to `MainWindowViewModel`.
4. Register the view model in `App.ConfigureServices` if it has no runtime arguments. Otherwise, have the calling view model create it.
5. Add all text in both languages. See [Add UI text](add-ui-text.md).
6. Use brushes from `Styles/AppTheme.axaml`.
7. Add XML documentation to all public members.
8. Add `Tests/ViewModels/ReportViewModelTests.cs` using the stubs.
9. Update the navigation table in [architecture.md](../architecture.md).
10. Run the checks in the [definition of done](../../AGENTS.md#definition-of-done).

Automated tests do not cover AXAML layout. Tell the user the page still needs a visual check.
