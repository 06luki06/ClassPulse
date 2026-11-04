using At.luki0606.ClassPulse.Data.Entities;
using At.luki0606.ClassPulse.Services;
using At.luki0606.ClassPulse.ViewModels.Dialogs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace At.luki0606.ClassPulse.ViewModels
{
    /// <summary>
    /// View model of the start page, which lists, creates and deletes classes.
    /// </summary>
    public partial class HomeViewModel : ViewModelBase
    {
        private readonly IClassService _classService;
        private readonly IDialogService _dialogService;
        private readonly IAssessmentService _assessmentService;

        private bool HasSelectedClass => SelectedClass != null;

        /// <summary>
        /// The class currently selected in the list.
        /// </summary>
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(GoToClassCommand))]
        [NotifyCanExecuteChangedFor(nameof(DeleteClassCommand))]
        private SchoolClass? _selectedClass;

        /// <summary>
        /// Gets the classes shown in the list.
        /// </summary>
        public ObservableCollection<SchoolClass> SchoolClasses { get; } = [];

        /// <summary>
        /// Initializes a new instance and starts loading the classes in the background.
        /// </summary>
        /// <param name="classService">The class service.</param>
        /// <param name="dialogService">The dialog service.</param>
        /// <param name="assessmentService">The assessment service passed to class pages.</param>
        public HomeViewModel(IClassService classService, IDialogService dialogService, IAssessmentService assessmentService)
        {
            _classService = classService;
            _dialogService = dialogService;
            _assessmentService = assessmentService;

            _ = InitializeAsync();
        }

        /// <summary>
        /// Loads the initial page data.
        /// </summary>
        /// <returns>A task that completes when loading has finished.</returns>
        [RelayCommand]
        public async Task InitializeAsync()
        {
            await LoadClassesAsync();
        }

        /// <summary>
        /// Reloads the classes and selects the first one, if any.
        /// </summary>
        /// <returns>A task that completes when loading has finished.</returns>
        [RelayCommand]
        public async Task LoadClassesAsync()
        {
            SchoolClasses.Clear();
            List<SchoolClass> classes = await _classService.GetAllClassesAsync();
            foreach (SchoolClass schoolClass in classes)
            {
                SchoolClasses.Add(schoolClass);
            }

            if (SchoolClasses.Any())
            {
                SelectedClass = SchoolClasses[0];
            }
        }

        /// <summary>
        /// Asks for a name and school year and creates a class if the user confirms with non-blank values.
        /// </summary>
        /// <returns>A task that completes when the dialog is closed and the class is saved.</returns>
        [RelayCommand]
        public async Task CreateClassAsync()
        {
            int currentYear = DateTime.Now.Year;

            InputDialogResult? result = await _dialogService.ShowInputDialogAsync(
                title: Resources.Resources.Dialog_NewClass_Title,
                message: Resources.Resources.Dialog_NewClass_Message,
                new InputField(Resources.Resources.Label_ClassName, $"{Resources.Resources.Label_for_example_abbr} 1a"),
                new InputField(Resources.Resources.Label_SchoolYear, $"{Resources.Resources.Label_for_example_abbr} {currentYear}/{currentYear + 1}", $"{currentYear}/{currentYear + 1}")
            );

            if (result is { IsConfirmed: true })
            {
                string name = result.ViewModel.GetValue(Resources.Resources.Label_ClassName);
                string schoolYear = result.ViewModel.GetValue(Resources.Resources.Label_SchoolYear);

                if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(schoolYear))
                {
                    SchoolClass newClass = await _classService.CreateClassAsync(name, schoolYear);
                    SchoolClasses.Add(newClass);
                    SelectedClass = newClass;
                }
            }
        }

        [RelayCommand(CanExecute = nameof(HasSelectedClass))]
        private async Task DeleteClassAsync()
        {
            if (SelectedClass == null)
            {
                return;
            }

            await _classService.DeleteClassAsync(SelectedClass.Id);
            SchoolClasses.Remove(SelectedClass);

            if (SchoolClasses.Any())
            {
                SelectedClass = SchoolClasses[0];
            }
            else
            {
                SelectedClass = null;
            }
        }

        [RelayCommand(CanExecute = nameof(HasSelectedClass))]
        private void GoToClass()
        {
            if (SelectedClass == null)
            {
                return;
            }

            ClassDetailViewModel classDetailVm = new(SelectedClass, _classService, _dialogService, _assessmentService);

            if (App.Current is App { Services: { } services })
            {
                MainWindowViewModel mainVm = services.GetRequiredService<MainWindowViewModel>();
                mainVm.NavigateToClassDetail(classDetailVm);
            }
        }

        [RelayCommand]
        private static void GoToSettings()
        {
            if (App.Current is App { Services: { } services })
            {
                MainWindowViewModel mainVm = services.GetRequiredService<MainWindowViewModel>();
                mainVm.NavigateToSettings();
            }
        }
    }
}
