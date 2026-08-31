using CommunityToolkit.Mvvm.ComponentModel;

namespace At.luki0606.ClassPulse.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        [ObservableProperty]
        private ObservableObject _currentView;

        public HomeViewModel HomeVm { get; }

        public MainWindowViewModel(HomeViewModel homeViewModel)
        {
            HomeVm = homeViewModel;

            _currentView = HomeVm;
        }
    }
}
