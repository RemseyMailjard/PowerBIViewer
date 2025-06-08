// FILE: ViewModels/MainViewModel.cs
using Microsoft.Extensions.DependencyInjection;
using PowerBIViewer.App.Commands;
using PowerBIViewer.App.Models;
using PowerBIViewer.App.Properties;
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

        public event EventHandler? ScreenshotRequested;
        private bool _isDarkMode;
        private string _statusText = "Klaar";
        private string? _selectedReportUrl;
        private bool _isLoading;
        private string? _selectedReportKey;

        public ObservableCollection<ReportDefinition> Reports { get; private set; } = [];
        public ObservableCollection<ReportDefinition> FavoriteReports { get; private set; } = [];

        public string StatusText { get => _statusText; set { _statusText = value; OnPropertyChanged(); } }
        public string? SelectedReportUrl { get => _selectedReportUrl; set { _selectedReportUrl = value; OnPropertyChanged(); } }
        public bool IsLoading { get => _isLoading; set { _isLoading = value; OnPropertyChanged(); } }

        public bool IsDarkMode
        {
            get => _isDarkMode;
            set { _isDarkMode = value; OnPropertyChanged(); UpdateThemeButton(); }
        }

        public string? SelectedReportKey { get => _selectedReportKey; set { _selectedReportKey = value; OnPropertyChanged(); } }
        public string ThemeButtonContent { get; private set; } = "🌙";
        public string ThemeButtonToolTip { get; private set; } = "Wissel naar Donker thema";

        public ICommand LoadReportCommand { get; }
        public ICommand LoadCommunityCommand { get; }
        public ICommand LoadNovyProCommand { get; }
        public ICommand ToggleThemeCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand OpenWidgetLauncherCommand { get; }
        public ICommand AboutCommand { get; }
        public ICommand ScreenshotCommand { get; }
        public ICommand OpenSettingsCommand { get; }
        public ICommand ToggleFavoriteCommand { get; }

        public MainViewModel()
        {
            LoadReportCommand = new RelayCommand(p => { });
            LoadCommunityCommand = new RelayCommand(p => { });
            LoadNovyProCommand = new RelayCommand(p => { });
            ToggleThemeCommand = new RelayCommand(p => { });
            RefreshCommand = new RelayCommand(p => { });
            OpenWidgetLauncherCommand = new RelayCommand(p => { });
            AboutCommand = new RelayCommand(p => { });
            ScreenshotCommand = new RelayCommand(p => { });
            OpenSettingsCommand = new RelayCommand(p => { });
            ToggleFavoriteCommand = new RelayCommand(p => { });
        }

        public MainViewModel(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
            LoadAndSetFavorites();

            ToggleFavoriteCommand = new RelayCommand(ExecuteToggleFavorite);
            LoadReportCommand = new RelayCommand(ExecuteLoadReport);
            LoadCommunityCommand = new RelayCommand(ExecuteLoadCommunity);
            LoadNovyProCommand = new RelayCommand(ExecuteLoadNovyPro);
            ToggleThemeCommand = new RelayCommand(p => IsDarkMode = !IsDarkMode);
            RefreshCommand = new RelayCommand(p => { /* Handled in View */ });
            OpenWidgetLauncherCommand = new RelayCommand(ExecuteOpenWidgetLauncher);
            AboutCommand = new RelayCommand(ExecuteShowAbout);
            ScreenshotCommand = new RelayCommand(p => ScreenshotRequested?.Invoke(this, EventArgs.Empty));
            OpenSettingsCommand = new RelayCommand(ExecuteOpenSettings);

            if (Reports.Any())
            {
                ExecuteLoadReport(Reports[0].Key);
            }
        }

        private void LoadAndSetFavorites()
        {
            var allReports = _reportRepository!.GetAll() ?? Enumerable.Empty<ReportDefinition>();

            if (Settings.Default.FavoriteReportKeys == null)
            {
                Settings.Default.FavoriteReportKeys = [];
            }

            foreach (var report in allReports)
            {
                report.IsFavorite = Settings.Default.FavoriteReportKeys.Contains(report.Key);
            }

            Reports = new(allReports);
            FavoriteReports = new(allReports.Where(r => r.IsFavorite));

            OnPropertyChanged(nameof(Reports));
            OnPropertyChanged(nameof(FavoriteReports));
        }

        private void ExecuteToggleFavorite(object? parameter)
        {
            if (parameter is ReportDefinition report)
            {
                report.IsFavorite = !report.IsFavorite;

                if (report.IsFavorite)
                {
                    if (!Settings.Default.FavoriteReportKeys.Contains(report.Key))
                    {
                        Settings.Default.FavoriteReportKeys.Add(report.Key);
                        FavoriteReports.Add(report);
                    }
                }
                else
                {
                    Settings.Default.FavoriteReportKeys.Remove(report.Key);
                    FavoriteReports.Remove(report);
                }

                Settings.Default.Save();
            }
        }

        private void ExecuteOpenSettings(object? p)
        {
            var settingsWindow = App.ServiceProvider?.GetService<SettingsWindow>();
            if (settingsWindow != null)
            {
                settingsWindow.Owner = Application.Current.MainWindow;
                settingsWindow.ShowDialog();
                LoadAndSetFavorites();
            }
        }

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
            SelectedReportUrl = "https://www.novypro.com/";
            SelectedReportKey = null;
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
            new WidgetLauncher().Show();
        }

        private void ExecuteShowAbout(object? parameter)
        {
            new AboutWindow().ShowDialog();
        }
    }
}