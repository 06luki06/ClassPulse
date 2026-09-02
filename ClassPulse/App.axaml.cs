using At.luki0606.ClassPulse.Data;
using At.luki0606.ClassPulse.Services;
using At.luki0606.ClassPulse.ViewModels;
using At.luki0606.ClassPulse.ViewModels.Dialogs;
using At.luki0606.ClassPulse.Views;
using At.luki0606.ClassPulse.Views.Dialogs;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Threading.Tasks;

namespace At.luki0606.ClassPulse
{
    public partial class App : Application
    {
        public IServiceProvider? Services { get; private set; }

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            Dispatcher.UIThread.UnhandledException += OnUiThreadUnhandledException;
            TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
            AppDomain.CurrentDomain.UnhandledException += OnAppDomainUnhandledException;

            ServiceCollection serviceCollection = new();
            ConfigureServices(serviceCollection);
            Services = serviceCollection.BuildServiceProvider();

            SettingsService settingsService = Services.GetRequiredService<SettingsService>();
            ApplySettings(settingsService.LoadSettings());

            using (IServiceScope scope = Services.CreateScope())
            {
                AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                dbContext.Database.EnsureCreated();
            }

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                MainWindowViewModel mainViewModel = Services.GetRequiredService<MainWindowViewModel>();
                desktop.MainWindow = new MainWindow
                {
                    DataContext = mainViewModel,
                };
            }

            base.OnFrameworkInitializationCompleted();
        }

        private static void ApplySettings(AppSettings settings)
        {
            if (Application.Current != null)
            {
                Application.Current.RequestedThemeVariant = settings.Theme switch
                {
                    "Dark" => Avalonia.Styling.ThemeVariant.Dark,
                    "Light" => Avalonia.Styling.ThemeVariant.Light,
                    _ => Avalonia.Styling.ThemeVariant.Default
                };
            }

            System.Globalization.CultureInfo culture = new(settings.Language);
            System.Threading.Thread.CurrentThread.CurrentCulture = culture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = culture;
            ClassPulse.Resources.Resources.Culture = culture;
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            string dbPath = Path.Combine(Utils.GetAppdataFolderPath(), "classpulse.db");
            services.AddDbContext<AppDbContext>(options => options.UseSqlite($"Data Source={dbPath}"));

            services.AddScoped<IClassService, ClassService>();
            services.AddScoped<IAssessmentService, AssessmentService>();
            services.AddScoped<IDialogService, AvaloniaDialogService>();
            services.AddSingleton<SettingsService>();

            services.AddTransient<HomeViewModel>();
            services.AddTransient<SettingsViewModel>();
            services.AddSingleton<MainWindowViewModel>();
        }

        private void OnUiThreadUnhandledException(object? sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            ShowErrorDialog("UI Thread Error", e.Exception);
        }

        private void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            e.SetObserved();
            Dispatcher.UIThread.Post(() =>
            {
                ShowErrorDialog("Async Task Error", e.Exception.InnerException ?? e.Exception);
            });
        }

        private void OnAppDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
            {
                Dispatcher.UIThread.Post(() =>
                {
                    ShowErrorDialog("Fatal Application Error", ex);
                });
            }
        }

        private async Task ShowErrorDialog(string title, Exception ex)
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop && desktop.MainWindow != null)
            {
                InputDialogWindow dialog = new()
                {
                    DataContext = new InputDialogViewModel(
                        title: $"⚠️ {title}",
                        message: $"{ClassPulse.Resources.Resources.General_ExceptionHasOccured}\n\n{ex.Message}",
                        fields: []
                        )
                };

                await dialog.ShowDialog(desktop.MainWindow);
            }
        }
    }
}