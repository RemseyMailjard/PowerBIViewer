// FILE: PowerBIViewer.App/App.xaml.cs (MET DESIGNER-CHECK)
using Microsoft.Extensions.DependencyInjection;
using PowerBIViewer.App.Services;
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

        // In App.xaml.cs -> ConfigureServices
        private static void ConfigureServices(IServiceCollection services)
        {
            // === SERVICES ===
            services.AddSingleton<IReportRepository, ReportRepository>();
            // ✨ NIEUW: Registreer de WidgetRepository. We geven de factory-methode mee
            // omdat de constructor een parameter (het bestandspad) nodig heeft.
            services.AddSingleton<IWidgetRepository>(provider =>
                new WidgetRepository("Data/widgets.json"));

            // === VIEWMODELS ===
            services.AddTransient<MainViewModel>();
            // ✨ NIEUW: Registreer de SettingsViewModel
            services.AddTransient<SettingsViewModel>();

            // === VIEWS ===
            services.AddSingleton<MainWindow>();
            // ✨ NIEUW: Registreer het SettingsWindow. Transient is goed, zodat je eventueel
            // een 'schoon' venster kunt openen als je dat later wilt.
            services.AddTransient<SettingsWindow>();
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