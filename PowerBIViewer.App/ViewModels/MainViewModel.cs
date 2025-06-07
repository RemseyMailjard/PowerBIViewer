// FILE: PowerBIViewer.App/ViewModels/MainViewModel.cs
using PowerBIViewer.App.Commands;
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
        // Event voor View-interactie
        public event EventHandler? ScreenshotRequested;

        // --- Private Fields ---
        private bool _isDarkMode;
        private string _statusText = "Klaar";
        private string? _selectedReportUrl;
        private bool _isLoading;
        private string? _selectedReportKey;

        // --- Public Properties ---
        public ObservableCollection<ReportDefinition> Reports { get; }
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

        public MainViewModel()
        {
            Reports = new ObservableCollection<ReportDefinition>(ReportRepository.GetAll());

            // Initialiseer alle commands
            LoadReportCommand = new RelayCommand(ExecuteLoadReport);
            LoadCommunityCommand = new RelayCommand(ExecuteLoadCommunity);
            LoadNovyProCommand = new RelayCommand(ExecuteLoadNovyPro);
            ToggleThemeCommand = new RelayCommand(ExecuteToggleTheme);
            RefreshCommand = new RelayCommand(p => { /* Handled in View */ });
            OpenWidgetLauncherCommand = new RelayCommand(ExecuteOpenWidgetLauncher);
            AboutCommand = new RelayCommand(ExecuteShowAbout);

            // ✨ GECORRIGEERD: De ScreenshotCommand wordt nu maar één keer correct geïnitialiseerd.
            ScreenshotCommand = new RelayCommand(p => ScreenshotRequested?.Invoke(this, EventArgs.Empty));

            // De tijdelijke command voor de nog niet geïmplementeerde feature.
            OpenSettingsCommand = new RelayCommand(p => MessageBox.Show("Instellingenvenster wordt later toegevoegd."));

            // Laad het eerste rapport bij het opstarten
            if (Reports.Any())
            {
                ExecuteLoadReport(Reports[0].Key);
            }
        }

        // --- Execute Methods ---
        private void ExecuteLoadReport(object? parameter)
        {
            IsLoading = true;
            var key = parameter as string;
            var report = ReportRepository.GetByKey(key ?? string.Empty);
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
            OnPropertyChanged(nameof(ThemeButtonContent));
            OnPropertyChanged(nameof(ThemeButtonToolTip));
        }

        private void ExecuteOpenWidgetLauncher(object? parameter)
        {
            var launcher = new WidgetLauncher();
            launcher.Show();
        }

        private void ExecuteShowAbout(object? parameter)
        {
            var aboutWindow = new AboutWindow();
            aboutWindow.ShowDialog();
        }
    }
}