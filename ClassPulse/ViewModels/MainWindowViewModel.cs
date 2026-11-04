using At.luki0606.ClassPulse.Services;
using CommunityToolkit.Mvvm.ComponentModel;

namespace At.luki0606.ClassPulse.ViewModels
{
    /// <summary>
    /// Holds the current page and performs navigation between pages.
    /// </summary>
    public partial class MainWindowViewModel : ViewModelBase
    {
        private readonly SettingsService _settingsService;

        /// <summary>
        /// The view model of the page currently shown.
        /// </summary>
        [ObservableProperty]
        private ObservableObject _currentView;

        /// <summary>
        /// Gets the view model of the start page, which is kept for the whole application lifetime.
        /// </summary>
        public HomeViewModel HomeVm { get; }

        /// <summary>
        /// Initializes a new instance that starts on the home page.
        /// </summary>
        /// <param name="homeViewModel">The view model of the start page.</param>
        /// <param name="settingsService">The service passed to the settings page.</param>
        public MainWindowViewModel(HomeViewModel homeViewModel, SettingsService settingsService)
        {
            HomeVm = homeViewModel;
            _settingsService = settingsService;

            _currentView = HomeVm;
        }

        /// <summary>
        /// Shows the start page.
        /// </summary>
        public void NavigateToHome()
        {
            CurrentView = HomeVm;
        }

        /// <summary>
        /// Shows a class page.
        /// </summary>
        /// <param name="classDetailViewModel">The view model of the class page.</param>
        public void NavigateToClassDetail(ClassDetailViewModel classDetailViewModel)
        {
            CurrentView = classDetailViewModel;
        }

        /// <summary>
        /// Shows a new settings page.
        /// </summary>
        public void NavigateToSettings()
        {
            CurrentView = new SettingsViewModel(_settingsService);
        }

        /// <summary>
        /// Shows a student page.
        /// </summary>
        /// <param name="studentDetailViewModel">The view model of the student page.</param>
        public void NavigateToStudentDetail(StudentDetailViewModel studentDetailViewModel)
        {
            CurrentView = studentDetailViewModel;
        }
    }
}
