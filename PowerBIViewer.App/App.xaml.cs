// FILE: PowerBIViewer.App/App.xaml.cs (MET DESIGNER-CHECK)
using Microsoft.Extensions.DependencyInjection;
using PowerBIViewer.App.ViewModels;
using PowerBIViewer.App.Views;
using System;
using System.ComponentModel; // ✨ TOEGEVOEGD: Nodig voor DesignerProperties
using System.Windows;

namespace PowerBIViewer.App
{
    public partial class App : Application
    {
        public static IServiceProvider? ServiceProvider { get; private set; }

        public App()
        {
            // ✨ BELANGRIJKE CHECK: Voer de DI-configuratie NIET uit als we in de designer-modus zijn.
            // De designer kan de complexe DI-keten niet opbouwen en crasht,
            // waardoor hij de resources (zoals de converters) niet kan vinden.
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                return;
            }

            // Deze code wordt alleen uitgevoerd wanneer de applicatie echt draait.
            var services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Deze methode blijft ongewijzigd
            services.AddSingleton<IReportRepository, ReportRepository>();
            services.AddTransient<MainViewModel>();
            services.AddSingleton<MainWindow>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // ✨ BELANGRIJKE CHECK: Zorg ervoor dat de ServiceProvider is aangemaakt.
            if (ServiceProvider == null) return;

            var mainWindow = ServiceProvider.GetService<MainWindow>();
            mainWindow?.Show();
        }
    }
}