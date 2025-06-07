// FILE: PowerBIViewer.App/ViewModels/MainViewModel.cs
using PowerBIViewer.App.Commands;
using PowerBIViewer.App.Views;
using PowerBIViewerApp;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Linq;

namespace PowerBIViewer.App.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        // Private fields voor de state van de applicatie
        private bool _isDarkMode;
        private string _statusText = "Klaar";
        private string? _selectedReportUrl;
        private bool _isLoading;

        // Public properties waar de View (XAML) aan kan binden
        public ObservableCollection<ReportDefinition> Reports { get; }
        public string StatusText { get => _statusText; set { _statusText = value; OnPropertyChanged(); } }
        public string? SelectedReportUrl { get => _selectedReportUrl; set { _selectedReportUrl = value; OnPropertyChanged(); } }
        public bool IsLoading { get => _isLoading; set { _isLoading = value; OnPropertyChanged(); } }
        public bool IsDarkMode { get => _isDarkMode; set { _isDarkMode = value; OnPropertyChanged(); UpdateThemeButton(); } }

        // Properties specifiek voor de themaknop, zodat de View geen logica nodig heeft
        public string ThemeButtonContent { get; private set; } = "🌙";
        public string ThemeButtonToolTip { get; private set; } = "Wissel naar Donker thema";

        // Commands voor de knoppen in de View
        public ICommand LoadReportCommand { get; }
        public ICommand LoadCommunityCommand { get; }
        public ICommand LoadNovyProCommand { get; }
        public ICommand ToggleThemeCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand OpenWidgetLauncherCommand { get; }
        public ICommand AboutCommand { get; }

        // Tijdelijke commands voor nog niet geïmplementeerde features
        public ICommand ScreenshotCommand { get; }
        public ICommand OpenSettingsCommand { get; }

        public MainViewModel()
        {
            // Laad de data
            Reports = new ObservableCollection<ReportDefinition>(ReportRepository.GetAll());

            // Initialiseer de commands met de logica die ze moeten uitvoeren
            LoadReportCommand = new RelayCommand(ExecuteLoadReport);
            LoadCommunityCommand = new RelayCommand(ExecuteLoadCommunity);
            LoadNovyProCommand = new RelayCommand(ExecuteLoadNovyPro);
            ToggleThemeCommand = new RelayCommand(ExecuteToggleTheme);
            RefreshCommand = new RelayCommand(p => { /* Logica komt in de View code-behind */ });
            OpenWidgetLauncherCommand = new RelayCommand(ExecuteOpenWidgetLauncher);
            AboutCommand = new RelayCommand(ExecuteShowAbout);

            ScreenshotCommand = new RelayCommand(p => MessageBox.Show("Screenshot-functie wordt later toegevoegd."));
            OpenSettingsCommand = new RelayCommand(p => MessageBox.Show("Instellingenvenster wordt later toegevoegd."));

            // Laad het eerste rapport bij opstarten
            if (Reports.Any())
            {
                ExecuteLoadReport(Reports[0].Key);
            }
        }

        // De logica die voorheen in de _Click handlers stond
        private void ExecuteLoadReport(object? parameter)
        {
            IsLoading = true;
            var key = parameter as string;
            var report = ReportRepository.GetByKey(key ?? string.Empty);
            if (report != null)
            {
                StatusText = $"Rapport '{report.Name}' wordt geladen...";
                SelectedReportUrl = report.Url;
            }
        }

        private void ExecuteLoadCommunity(object? parameter)
        {
            IsLoading = true;
            StatusText = "Rapport 'Power BI Community Gallery' wordt geladen...";
            SelectedReportUrl = "https://community.powerbi.com/t5/Data-Stories-Gallery/bd-p/DataStoriesGallery";
        }

        private void ExecuteLoadNovyPro(object? parameter)
        {
            IsLoading = true;
            StatusText = "Rapport 'NovyPro Explore' wordt geladen...";
            SelectedReportUrl = "https://www.novypro.com/explore_projects";
        }

        private void ExecuteToggleTheme(object? parameter)
        {
            IsDarkMode = !IsDarkMode;
        }

        private void UpdateThemeButton()
        {
            if (IsDarkMode)
            {
                ThemeButtonContent = "☀️";
                ThemeButtonToolTip = "Wissel naar Licht thema";
            }
            else
            {
                ThemeButtonContent = "🌙";
                ThemeButtonToolTip = "Wissel naar Donker thema";
            }
            // Informeer de View dat deze properties ook gewijzigd zijn
            OnPropertyChanged(nameof(ThemeButtonContent));
            OnPropertyChanged(nameof(ThemeButtonToolTip));
        }

        private void ExecuteOpenWidgetLauncher(object? parameter)
        {
            try
            {
                WidgetLauncher launcher = new WidgetLauncher();
                // We kunnen geen Owner instellen vanuit de VM, dit blijft in de View
                launcher.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kon de Widget Launcher niet openen:\n{ex.Message}", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteShowAbout(object? parameter)
        {
            var aboutWindow = new AboutWindow();
            aboutWindow.ShowDialog();
        }
    }
}