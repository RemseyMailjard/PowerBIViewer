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
        // ✨ GEWIJZIGD: Nullable gemaakt (? toegevoegd) om de designer-constructor te ondersteunen.
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

        // ✨ NIEUW: Een parameterloze constructor specifiek voor de XAML Designer.
        // Deze wordt aangeroepen door `d:DesignInstance IsDesignTimeCreatable=True` in de XAML.
        public MainViewModel()
        {
            // Initialiseer de collectie als leeg om null-reference exceptions in de designer te voorkomen.
            Reports = new ObservableCollection<ReportDefinition>();

            // Initialiseer de commands met lege acties om crashes te voorkomen.
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

        // ✨ De "echte" constructor die door de Dependency Injection container wordt gebruikt.
        public MainViewModel(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;

            Reports = new ObservableCollection<ReportDefinition>(_reportRepository.GetAll() ?? Enumerable.Empty<ReportDefinition>());

            // Initialiseer de commands met de daadwerkelijke logica.
            LoadReportCommand = new RelayCommand(ExecuteLoadReport);
            LoadCommunityCommand = new RelayCommand(ExecuteLoadCommunity);
            LoadNovyProCommand = new RelayCommand(ExecuteLoadNovyPro);
            ToggleThemeCommand = new RelayCommand(ExecuteToggleTheme);
            RefreshCommand = new RelayCommand(p => { /* Handled in View */ });
            OpenWidgetLauncherCommand = new RelayCommand(ExecuteOpenWidgetLauncher);
            AboutCommand = new RelayCommand(ExecuteShowAbout);
            ScreenshotCommand = new RelayCommand(p => ScreenshotRequested?.Invoke(this, EventArgs.Empty));
            OpenSettingsCommand = new RelayCommand(p => MessageBox.Show("Instellingenvenster wordt later toegevoegd."));

            if (Reports.Any())
            {
                ExecuteLoadReport(Reports[0].Key);
            }
        }

        // --- Execute Methods (gebruiken nu de _reportRepository instance) ---
        private void ExecuteLoadReport(object? parameter)
        {
            IsLoading = true;
            var key = parameter as string;
            // De null-forgiving operator (!) is hier veilig omdat deze methode alleen wordt aangeroepen vanuit de DI-constructor.
            var report = _reportRepository!.GetByKey(key ?? string.Empty);
            if (report != null)
            {
                StatusText = $"Rapport '{report.Name}' wordt geladen...";
                SelectedReportUrl = report.Url;
                SelectedReportKey = report.Key;
            }
        }

        // ... de rest van de methodes blijft hetzelfde ...
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