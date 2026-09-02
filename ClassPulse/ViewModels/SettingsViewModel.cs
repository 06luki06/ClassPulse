using At.luki0606.ClassPulse.Data;
using At.luki0606.ClassPulse.Services;
using Avalonia;
using Avalonia.Styling;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace At.luki0606.ClassPulse.ViewModels
{
    public partial class SettingsViewModel : ViewModelBase
    {
        private readonly SettingsService _settingsService;

        [ObservableProperty]
        private ThemeOption? _selectedThemeOption;

        [ObservableProperty]
        private LanguageOption? _selectedLanguageOption;

        public List<ThemeOption> AvailableThemes { get; private set; }
        public List<LanguageOption> AvailableLanguages { get; private set; }

        public SettingsViewModel(SettingsService settingsService)
        {
            _settingsService = settingsService;

            AvailableThemes = BuildThemeOptions();
            AvailableLanguages = BuildLanguageOptions();

            ThemeVariant currentTheme = Application.Current?.RequestedThemeVariant ?? ThemeVariant.Default;
            _selectedThemeOption = AvailableThemes.FirstOrDefault(t => t.Variant == currentTheme) ?? AvailableThemes[0];

            string currentCode = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;
            _selectedLanguageOption = AvailableLanguages.FirstOrDefault(l => l.Code == currentCode) ?? AvailableLanguages[0];
        }

        partial void OnSelectedThemeOptionChanged(ThemeOption? value)
        {
            if (Application.Current != null && value != null)
            {
                Application.Current.RequestedThemeVariant = value.Variant;
                PersistCurrentSettings();
            }
        }

        partial void OnSelectedLanguageOptionChanged(LanguageOption? value)
        {
            if (value == null)
            {
                return;
            }

            CultureInfo cultureInfo = new(value.Code);
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;
            Resources.Resources.Culture = cultureInfo;

            UpdateOptionLabels();
            PersistCurrentSettings();
        }

        private void PersistCurrentSettings()
        {
            string themeStr = SelectedThemeOption?.Variant == ThemeVariant.Dark ? "Dark" :
                              SelectedThemeOption?.Variant == ThemeVariant.Light ? "Light" : "System";

            string langStr = SelectedLanguageOption?.Code ?? "de";

            _settingsService.SaveSettings(new AppSettings
            {
                Theme = themeStr,
                Language = langStr
            });
        }

        private void UpdateOptionLabels()
        {
            AvailableThemes = BuildThemeOptions();
            AvailableLanguages = BuildLanguageOptions();

            OnPropertyChanged(nameof(AvailableThemes));
            OnPropertyChanged(nameof(AvailableLanguages));

            ThemeVariant? currentTheme = Application.Current?.RequestedThemeVariant;
            SelectedThemeOption = AvailableThemes.FirstOrDefault(t => t.Variant == currentTheme) ?? AvailableThemes[0];

            string currentCode = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;
            SelectedLanguageOption = AvailableLanguages.FirstOrDefault(l => l.Code == currentCode) ?? AvailableLanguages[0];

            OnPropertyChanged(nameof(SelectedThemeOption));
            OnPropertyChanged(nameof(SelectedLanguageOption));
        }

        private static List<ThemeOption> BuildThemeOptions()
        {
            return [
            new(ThemeVariant.Default, Resources.Resources.Label_System),
            new(ThemeVariant.Light, Resources.Resources.Label_Light),
            new(ThemeVariant.Dark, Resources.Resources.Label_Dark)
        ];
        }

        private static List<LanguageOption> BuildLanguageOptions()
        {
            return [
            new("de", Resources.Resources.Label_German),
            new("en", Resources.Resources.Label_English)
        ];
        }

        [RelayCommand]
        private static void BackToHome()
        {
            if (App.Current is App { Services: { } services })
            {
                MainWindowViewModel mainVm = services.GetRequiredService<MainWindowViewModel>();
                mainVm.NavigateToHome();
            }
        }
    }

    public record ThemeOption(ThemeVariant Variant, string Label)
    {
        public override string ToString()
        {
            return Label;
        }
    }

    public record LanguageOption(string Code, string Label)
    {
        public override string ToString()
        {
            return Label;
        }
    }
}