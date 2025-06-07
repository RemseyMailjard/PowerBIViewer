// FILE: PowerBIViewer.App/ViewModels/MainViewModel.cs
using Microsoft.Extensions.DependencyInjection; // ✨ TOEGEVOEGD
using PowerBIViewer.App.Commands;
using PowerBIViewer.App.Services;
using PowerBIViewer.App.Views;
using PowerBIViewerApp;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace PowerBIViewer.App.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IReportRepository? _reportRepository;

        // --- Event en Private Fields ---
        public event EventHandler? ScreenshotRequested;
        private bool _isDarkMode;
        private string _statusText = "Klaar";
        private string? _selectedReportUrl;
        private bool _isLoading;
        private string? _selectedReportKey;

        // --- Public Properties ---
        public ObservableCollection<ReportDefinition> Reports { get; private set; }
        public string StatusText { get => _statusText; set { _statusText = value; OnPropertyChanged(); } }
        public string? SelectedReportUrl { get => _selectedReportUrl; set { _selectedReportUrl = value; OnPropertyChanged(); } }
        public bool IsLoading { get => _isLoading; set { _isLoading = value; OnPropertyChanged(); } }
        public bool IsDarkMode { get => _isDarkMode; set { _isDarkMode = value; OnPropertyChanged(); UpdateThemeButton(); } }
        public string? SelectedReportKey { get => _selectedReportKey; set { _selectedReportKey = value; OnPropertyChanged(); } }
        public string ThemeButtonContent { get; private set; } = "🌙";
        public string ThemeButtonToolTip { get; private set; } = "Wissel naar Donker thema";

        // --- Commands ---
        public ICommand LoadReportCommand { get; }
        public ICommand LoadCommunityCommand { get; }
        public ICommand LoadNovyProCommand { get; }
        public ICommand ToggleThemeCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand OpenWidgetLauncherCommand { get; }
        public ICommand AboutCommand { get; }
        public ICommand ScreenshotCommand { get; }
        public ICommand OpenSettingsCommand { get; }

        // Constructor voor de XAML Designer
        public MainViewModel()
        {
            Reports = new ObservableCollection<ReportDefinition>();
            LoadReportCommand = new RelayCommand(p => { });
            LoadCommunityCommand = new RelayCommand(p => { });
            LoadNovyProCommand = new RelayCommand(p => { });
            ToggleThemeCommand = new RelayCommand(p => { });
            RefreshCommand = new RelayCommand(p => { });
            OpenWidgetLauncherCommand = new RelayCommand(p => { });
            AboutCommand = new RelayCommand(p => { });
            ScreenshotCommand = new RelayCommand(p => { });
            OpenSettingsCommand = new RelayCommand(p => { });
        }

        // De "echte" constructor die door DI wordt gebruikt.
        public MainViewModel(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
            Reports = new ObservableCollection<ReportDefinition>(_reportRepository.GetAll() ?? Enumerable.Empty<ReportDefinition>());

            LoadReportCommand = new RelayCommand(ExecuteLoadReport);
            LoadCommunityCommand = new RelayCommand(ExecuteLoadCommunity);
            LoadNovyProCommand = new RelayCommand(ExecuteLoadNovyPro);
            ToggleThemeCommand = new RelayCommand(ExecuteToggleTheme);
            RefreshCommand = new RelayCommand(p => { /* Handled in View */ });
            OpenWidgetLauncherCommand = new RelayCommand(ExecuteOpenWidgetLauncher);
            AboutCommand = new RelayCommand(ExecuteShowAbout);
            ScreenshotCommand = new RelayCommand(p => ScreenshotRequested?.Invoke(this, EventArgs.Empty));

            // ✨ GEWIJZIGD: De command is nu gekoppeld aan de juiste methode.
            OpenSettingsCommand = new RelayCommand(ExecuteOpenSettings);

            if (Reports.Any())
            {
                ExecuteLoadReport(Reports[0].Key);
            }
        }

        // ✨ NIEUWE METHODE: De logica voor het openen van het instellingenvenster.
        private void ExecuteOpenSettings(object? p)
        {
            // Vraag een SettingsWindow aan de DI container.
            // Dit werkt omdat we de ServiceProvider static hebben gemaakt in App.xaml.cs.
            var settingsWindow = App.ServiceProvider?.GetService<SettingsWindow>();
            if (settingsWindow != null)
            {
                // Stel de eigenaar in zodat het instellingenvenster boven het hoofdvenster verschijnt.
                settingsWindow.Owner = Application.Current.MainWindow;
                // ShowDialog() opent het venster modaal, wat betekent dat de gebruiker
                // eerst dit venster moet sluiten voordat hij terug kan naar het hoofdvenster.
                settingsWindow.ShowDialog();

                // Na het sluiten van het instellingenvenster (nadat er mogelijk op 'Opslaan' is geklikt),
                // laden we de rapporten opnieuw om eventuele wijzigingen in de UI te tonen.
                var freshReports = _reportRepository!.GetAll();
                Reports.Clear();
                foreach (var report in freshReports)
                {
                    Reports.Add(report);
                }
            }
        }

        // --- Bestaande Execute Methods ---
        private void ExecuteLoadReport(object? parameter)
        {
            IsLoading = true;
            var key = parameter as string;
            var report = _reportRepository!.GetByKey(key ?? string.Empty);
            if (report != null)
            {
                StatusText = $"Rapport '{report.Name}' wordt geladen...";
                SelectedReportUrl = report.Url;
                SelectedReportKey = report.Key;
            }
        }

        private void ExecuteLoadCommunity(object? parameter)
        {
            IsLoading = true;
            StatusText = "Rapport 'Power BI Community Gallery' wordt geladen...";
            SelectedReportUrl = "https://community.powerbi.com/t5/Data-Stories-Gallery/bd-p/DataStoriesGallery";
            SelectedReportKey = null;
        }

        private void ExecuteLoadNovyPro(object? parameter)
        {
            IsLoading = true;
            StatusText = "Rapport 'NovyPro Explore' wordt geladen...";
            SelectedReportUrl = "https://www.novypro.com/explore_projects";
            SelectedReportKey = null;
        }

        private void ExecuteToggleTheme(object? parameter) { IsDarkMode = !IsDarkMode; }
        private void UpdateThemeButton()
        {
            if (IsDarkMode) { ThemeButtonContent = "☀️"; ThemeButtonToolTip = "Wissel naar Licht thema"; }
            else { ThemeButtonContent = "🌙"; ThemeButtonToolTip = "Wissel naar Donker thema"; }
            OnPropertyChanged(nameof(ThemeButtonContent));
            OnPropertyChanged(nameof(ThemeButtonToolTip));
        }
        private void ExecuteOpenWidgetLauncher(object? parameter) { new WidgetLauncher().Show(); }
        private void ExecuteShowAbout(object? parameter) { new AboutWindow().ShowDialog(); }
    }
}