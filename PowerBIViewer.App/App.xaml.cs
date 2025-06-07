// FILE: PowerBIViewer.App/App.xaml.cs (CONTROLEER DEZE VERSIE)
using Microsoft.Extensions.DependencyInjection;
using PowerBIViewer.App.Services;
using PowerBIViewer.App.ViewModels;
using PowerBIViewer.App.Views;
using System;
using System.ComponentModel; // Nodig voor DesignerProperties
using System.Windows;

namespace PowerBIViewer.App
{
    public partial class App : Application
    {
        public static IServiceProvider? ServiceProvider { get; private set; }

        public App()
        {
            // BELANGRIJKE CHECK: Als de code wordt uitgevoerd door de designer,
            // sla dan de complexe DI-configuratie over.
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                return;
            }

            // Deze code wordt alleen uitgevoerd als de app ECHT draait.
            var services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IReportRepository, ReportRepository>();
            services.AddSingleton<IWidgetRepository>(provider => new WidgetRepository("Data/widgets.json"));
            services.AddTransient<MainViewModel>();
            services.AddTransient<SettingsViewModel>();
            services.AddSingleton<MainWindow>();
            services.AddTransient<SettingsWindow>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Veiligheidscheck voor de designer
            if (ServiceProvider == null) return;

            var mainWindow = ServiceProvider.GetService<MainWindow>();
            mainWindow?.Show();
        }
    }
}