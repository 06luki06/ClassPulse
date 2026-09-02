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
using ResourcesTxt = At.luki0606.ClassPulse.Resources;

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

        private static void ConfigureServices(IServiceCollection services)
        {
            string dbPath = Path.Combine(GetAppdataFolderPath(), "classpulse.db");
            services.AddDbContext<AppDbContext>(options => options.UseSqlite($"Data Source={dbPath}"));

            services.AddScoped<IClassService, ClassService>();
            services.AddScoped<IAssessmentService, AssessmentService>();
            services.AddScoped<IDialogService, AvaloniaDialogService>();

            services.AddTransient<HomeViewModel>();
            services.AddSingleton<MainWindowViewModel>();
        }

        private static string GetAppdataFolderPath()
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string folderPath = Path.Combine(appDataPath, "ClassPulse");
            Directory.CreateDirectory(folderPath);

            return folderPath;
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
                        message: $"{ResourcesTxt.Resources.General_ExceptionHasOccured}\n\n{ex.Message}",
                        fields: []
                        )
                };

                await dialog.ShowDialog(desktop.MainWindow);
            }
        }
    }
}