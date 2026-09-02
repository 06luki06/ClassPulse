using At.luki0606.ClassPulse.Services;
using CommunityToolkit.Mvvm.ComponentModel;

namespace At.luki0606.ClassPulse.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        private readonly SettingsService _settingsService;

        [ObservableProperty]
        private ObservableObject _currentView;

        public HomeViewModel HomeVm { get; }

        public MainWindowViewModel(HomeViewModel homeViewModel, SettingsService settingsService)
        {
            HomeVm = homeViewModel;
            _settingsService = settingsService;

            _currentView = HomeVm;
        }

        public void NavigateToHome()
        {
            CurrentView = HomeVm;
        }

        public void NavigateToClassDetail(ClassDetailViewModel classDetailViewModel)
        {
            CurrentView = classDetailViewModel;
        }

        public void NavigateToSettings()
        {
            CurrentView = new SettingsViewModel(_settingsService);
        }

        public void NavigateToStudentDetail(StudentDetailViewModel studentDetailViewModel)
        {
            CurrentView = studentDetailViewModel;
        }
    }
}
