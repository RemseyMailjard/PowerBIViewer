// FILE: PowerBIViewer.App/App.xaml.cs
using Microsoft.Extensions.DependencyInjection;
using PowerBIViewer.App.Services;
using PowerBIViewer.App.ViewModels;
using PowerBIViewer.App.Views;
using PowerBIViewerApp; // ✨ TOEGEVOEGD: Nodig voor de WidgetLauncher namespace
using System;
using System.ComponentModel;
using System.Windows;

namespace PowerBIViewer.App
{
    public partial class App : Application
    {
        public static IServiceProvider? ServiceProvider { get; private set; }

        public App()
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                return;
            }

            var services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            // === SERVICES ===
            services.AddSingleton<IReportRepository, ReportRepository>();
            services.AddSingleton<IWidgetRepository>(provider => new WidgetRepository("Data/widgets.json"));

            // === VIEWMODELS ===
            services.AddTransient<MainViewModel>();
            services.AddTransient<SettingsViewModel>();
            // ✨ NIEUW: Registreer de WidgetLauncherViewModel
            services.AddTransient<WidgetLauncherViewModel>();

            // === VIEWS ===
            services.AddSingleton<MainWindow>();
            services.AddTransient<SettingsWindow>();
            // ✨ NIEUW: Registreer de WidgetLauncher. Transient is prima.
            services.AddTransient<WidgetLauncher>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            if (ServiceProvider == null) return;

            var mainWindow = ServiceProvider.GetService<MainWindow>();
            mainWindow?.Show();
        }
    }
}