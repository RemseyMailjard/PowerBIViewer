// FILE: PowerBIViewer.App/Views/MainWindow.xaml.cs
using Microsoft.Web.WebView2.Core;
using PowerBIViewer.App.Helpers;
using PowerBIViewer.App.ViewModels;
using PowerBIViewerApp;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace PowerBIViewer.App.Views
{
    public partial class MainWindow : Window
    {
        private MainViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            RestoreWindowState();

            // 1. Maak de ViewModel aan en stel deze in als DataContext
            _viewModel = new MainViewModel();
            this.DataContext = _viewModel;
            _viewModel.ScreenshotRequested += OnScreenshotRequested;

            // Luister naar property changes op de ViewModel
            _viewModel.PropertyChanged += ViewModel_PropertyChanged;

            // Koppel events die de View moet afhandelen
            this.Closing += MainWindow_Closing;
            this.KeyDown += MainWindow_KeyDown;
            PowerBIWebView.NavigationCompleted += PowerBIWebView_NavigationCompleted;

            // Pas het thema direct toe bij het opstarten
            ApplyTheme(_viewModel.IsDarkMode);
        }

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            // Reageer op specifieke wijzigingen in de ViewModel
            if (e.PropertyName == nameof(MainViewModel.SelectedReportUrl))
            {
                PowerBIWebView.Source = !string.IsNullOrEmpty(_viewModel.SelectedReportUrl)
                    ? new Uri(_viewModel.SelectedReportUrl)
                    : null;
            }
            else if (e.PropertyName == nameof(MainViewModel.IsDarkMode))
            {
                ApplyTheme(_viewModel.IsDarkMode);
            }
        }

        private async void OnScreenshotRequested(object? sender, EventArgs e)
        {
            var saveDialog = new Microsoft.Win32.SaveFileDialog
            {
                FileName = $"Screenshot-{DateTime.Now:yyyyMMdd-HHmmss}.png",
                Filter = "PNG Image|*.png"
            };

            if (saveDialog.ShowDialog() == true)
            {
                try
                {
                    using (var stream = System.IO.File.Create(saveDialog.FileName))
                    {
                        // Zorg dat de WebView2 control is geïnitialiseerd
                        await PowerBIWebView.EnsureCoreWebView2Async();
                        await PowerBIWebView.CoreWebView2.CapturePreviewAsync(
                            Microsoft.Web.WebView2.Core.CoreWebView2CapturePreviewImageFormat.Png, stream);
                    }
                    MessageBox.Show($"Screenshot opgeslagen in:\n{saveDialog.FileName}", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Kon geen screenshot maken:\n{ex.Message}", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void ApplyTheme(bool isDarkMode)
        {
            DwmApiHelper.SetTitleBarTheme(this, isDarkMode);

            // Verwijder het oude thema
            var existingTheme = Application.Current.Resources.MergedDictionaries
                .FirstOrDefault(d => d.Source != null && d.Source.OriginalString.Contains("Mode.xaml"));
            if (existingTheme != null)
            {
                Application.Current.Resources.MergedDictionaries.Remove(existingTheme);
            }

            // Voeg het nieuwe thema toe
            string newThemeFileName = isDarkMode ? "Themes/DarkMode.xaml" : "Themes/LightMode.xaml";
            var newTheme = new ResourceDictionary { Source = new Uri(newThemeFileName, UriKind.Relative) };
            Application.Current.Resources.MergedDictionaries.Add(newTheme);
        }

        // --- Event Handlers die in de View blijven ---

        private void PowerBIWebView_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            _viewModel.IsLoading = false; // Verberg laad-indicator
            if (e.IsSuccess)
            {
                var reportName = ReportRepository.GetAll().FirstOrDefault(r => r.Url == PowerBIWebView.Source.AbsoluteUri)?.Name ?? "Externe pagina";
                _viewModel.StatusText = $"Rapport '{reportName}' succesvol geladen.";
            }
            else
            {
                _viewModel.StatusText = $"Fout bij het laden van de pagina.";
            }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            if (PowerBIWebView != null && PowerBIWebView.CoreWebView2 != null)
            {
                PowerBIWebView.Reload();
            }
        }

        // --- Vensterbeheer (blijft in de View) ---
        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            // ... (Deze code blijft exact hetzelfde als voorheen)
            if (this.WindowState == WindowState.Maximized)
            {
                Properties.Settings.Default.WindowTop = this.RestoreBounds.Top;
                Properties.Settings.Default.WindowLeft = this.RestoreBounds.Left;
                Properties.Settings.Default.WindowHeight = this.RestoreBounds.Height;
                Properties.Settings.Default.WindowWidth = this.RestoreBounds.Width;
            }
            else
            {
                Properties.Settings.Default.WindowTop = this.Top;
                Properties.Settings.Default.WindowLeft = this.Left;
                Properties.Settings.Default.WindowHeight = this.Height;
                Properties.Settings.Default.WindowWidth = this.Width;
            }
            Properties.Settings.Default.WindowState = this.WindowState.ToString();
            Properties.Settings.Default.Save();
        }

        private void RestoreWindowState()
        {
            // ... (Deze code blijft exact hetzelfde als voorheen)
            if (Properties.Settings.Default.WindowWidth > 0 && Properties.Settings.Default.WindowHeight > 0)
            {
                this.Top = Properties.Settings.Default.WindowTop;
                this.Left = Properties.Settings.Default.WindowLeft;
                this.Height = Properties.Settings.Default.WindowHeight;
                this.Width = Properties.Settings.Default.WindowWidth;

                if (Enum.TryParse(Properties.Settings.Default.WindowState, out WindowState state))
                {
                    this.WindowState = state;
                }
            }
        }

        private void ToggleFullScreen_Click(object sender, RoutedEventArgs e) => ToggleFullScreen();
        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F11) ToggleFullScreen();
        }

        private void ToggleFullScreen()
        {
            // ... (Deze code blijft exact hetzelfde als voorheen)
            if (this.WindowState == WindowState.Maximized && this.WindowStyle == WindowStyle.None)
            {
                this.WindowState = WindowState.Normal;
                this.WindowStyle = WindowStyle.SingleBorderWindow;
                this.ResizeMode = ResizeMode.CanResize;
                FullScreenButton.Content = "⛶";
                FullScreenButton.ToolTip = "Volledig scherm (F11)";
            }
            else
            {
                this.WindowState = WindowState.Maximized;
                this.WindowStyle = WindowStyle.None;
                this.ResizeMode = ResizeMode.NoResize;
                FullScreenButton.Content = "⤡";
                FullScreenButton.ToolTip = "Verlaat volledig scherm (F11)";
            }
        }

    }
}