using At.luki0606.ClassPulse.Data.Entities;
using At.luki0606.ClassPulse.Services;
using At.luki0606.ClassPulse.ViewModels.Dialogs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace At.luki0606.ClassPulse.ViewModels
{
    public partial class HomeViewModel : ViewModelBase
    {
        private readonly IClassService _classService;
        private readonly IDialogService _dialogService;

        private bool HasSelectedClass => SelectedClass != null;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(GoToClassCommand))]
        [NotifyCanExecuteChangedFor(nameof(DeleteClassCommand))]
        private SchoolClass? _selectedClass;

        public ObservableCollection<SchoolClass> SchoolClasses { get; } = [];

        public HomeViewModel(IClassService classService, IDialogService dialogService)
        {
            _classService = classService;
            _dialogService = dialogService;

            _ = InitializeAsync();
        }

        [RelayCommand]
        public async Task InitializeAsync()
        {
            await LoadClassesAsync();
        }

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
            //Navigate
        }
    }
}
