// FILE: PowerBIViewer.App/Views/MainWindow.xaml.cs
using Microsoft.Web.WebView2.Core;
using PowerBIViewer.App.Helpers;
using PowerBIViewer.App.ViewModels;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.IO;

namespace PowerBIViewer.App.Views
{
    public partial class MainWindow : Window
    {
        // ✨ GEWIJZIGD: Het field is nu 'readonly' omdat het alleen in de constructor wordt ingesteld.
        private readonly MainViewModel _viewModel;

        // ✨ GEWIJZIGD: De constructor heeft nu een parameter!
        // De DI container zal automatisch een MainViewModel aanleveren wanneer het een MainWindow moet maken.
        public MainWindow(MainViewModel viewModel)
        {
            InitializeComponent();
            RestoreWindowState();

            // ✨ GEWIJZIGD: De regel '_viewModel = new MainViewModel();' is VERWIJDERD.
            // We gebruiken de ViewModel die we hebben ontvangen als parameter.
            _viewModel = viewModel;
            this.DataContext = _viewModel;

            // De rest van de constructor blijft exact hetzelfde.
            _viewModel.ScreenshotRequested += OnScreenshotRequested;
            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
            this.Closing += MainWindow_Closing;
            this.KeyDown += MainWindow_KeyDown;
            PowerBIWebView.NavigationCompleted += PowerBIWebView_NavigationCompleted;
            ApplyTheme(_viewModel.IsDarkMode);
        }

        // --- Deze methode hoeft niet te veranderen, het gebruikt de _viewModel die al is ingesteld ---
        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MainViewModel.SelectedReportUrl) && PowerBIWebView != null)
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

        // --- Deze methode hoeft niet te veranderen ---
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
                    await PowerBIWebView.EnsureCoreWebView2Async();
                    using (var stream = File.Create(saveDialog.FileName))
                    {
                        await PowerBIWebView.CoreWebView2.CapturePreviewAsync(
                            CoreWebView2CapturePreviewImageFormat.Png, stream);
                    }
                    MessageBox.Show($"Screenshot opgeslagen in:\n{saveDialog.FileName}", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Kon geen screenshot maken:\n{ex.Message}", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // --- Deze methode hoeft niet te veranderen ---
        private void ApplyTheme(bool isDarkMode)
        {
            DwmApiHelper.SetTitleBarTheme(this, isDarkMode);
            var existingTheme = Application.Current.Resources.MergedDictionaries.FirstOrDefault(d => d.Source != null && d.Source.OriginalString.Contains("Mode.xaml"));
            if (existingTheme != null)
            {
                Application.Current.Resources.MergedDictionaries.Remove(existingTheme);
            }
            string newThemeFileName = isDarkMode ? "Themes/DarkMode.xaml" : "Themes/LightMode.xaml";
            var newTheme = new ResourceDictionary { Source = new Uri(newThemeFileName, UriKind.Relative) };
            Application.Current.Resources.MergedDictionaries.Add(newTheme);
        }

        // --- Event Handlers die in de View blijven ---
        private void PowerBIWebView_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            _viewModel.IsLoading = false;
            // ✨ VERBETERING: In plaats van de static ReportRepository, gebruiken we de lijst uit de ViewModel.
            if (e.IsSuccess && PowerBIWebView.Source != null)
            {
                var reportName = _viewModel.Reports.FirstOrDefault(r => r.Url == PowerBIWebView.Source.AbsoluteUri)?.Name ?? "Externe pagina";
                _viewModel.StatusText = $"Rapport '{reportName}' succesvol geladen.";
            }
            else
            {
                _viewModel.StatusText = $"Fout bij het laden van de pagina.";
            }
        }

        // --- De rest van de methodes blijft 100% ongewijzigd ---
        private void Refresh_Click(object sender, RoutedEventArgs e) { /* ... */ }
        private void MainWindow_Closing(object sender, CancelEventArgs e) { /* ... */ }
        private void RestoreWindowState() { /* ... */ }
        private void ToggleFullScreen_Click(object sender, RoutedEventArgs e) { /* ... */ }
        private void MainWindow_KeyDown(object sender, KeyEventArgs e) { /* ... */ }
        private void ToggleFullScreen() { /* ... */ }
    }
}