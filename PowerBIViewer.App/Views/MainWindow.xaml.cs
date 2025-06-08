// FILE: PowerBIViewer.App/Views/MainWindow.xaml.cs (CORRECTE, SCHONE VERSIE)
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
        private readonly MainViewModel _viewModel;

        public MainWindow(MainViewModel viewModel)
        {
            InitializeComponent();
            RestoreWindowState();

            _viewModel = viewModel;
            this.DataContext = _viewModel;

            _viewModel.ScreenshotRequested += OnScreenshotRequested;
            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
            this.Closing += MainWindow_Closing;
            this.KeyDown += MainWindow_KeyDown;
            PowerBIWebView.NavigationCompleted += PowerBIWebView_NavigationCompleted;
            ApplyTheme(_viewModel.IsDarkMode);
        }

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

        private void PowerBIWebView_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            _viewModel.IsLoading = false;
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

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            if (PowerBIWebView != null && PowerBIWebView.CoreWebView2 != null)
            {
                PowerBIWebView.Reload();
            }
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
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