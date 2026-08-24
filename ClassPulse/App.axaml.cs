using At.luki0606.ClassPulse.Data;
using At.luki0606.ClassPulse.ViewModels;
using At.luki0606.ClassPulse.Views;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Linq;

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
                // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
                // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
                DisableAvaloniaDataAnnotationValidation();
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
            services.AddTransient<MainWindowViewModel>();
        }

        private static string GetAppdataFolderPath()
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string folderPath = Path.Combine(appDataPath, "ClassPulse");
            Directory.CreateDirectory(folderPath);

            return folderPath;
        }

        private static void DisableAvaloniaDataAnnotationValidation()
        {
            // Get an array of plugins to remove
            DataAnnotationsValidationPlugin[] dataValidationPluginsToRemove =
                [.. BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>()];

            // remove each entry found
            foreach (DataAnnotationsValidationPlugin plugin in dataValidationPluginsToRemove)
            {
                BindingPlugins.DataValidators.Remove(plugin);
            }
        }
    }
}