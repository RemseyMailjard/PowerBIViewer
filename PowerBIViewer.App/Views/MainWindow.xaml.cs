// using-statements zoals je ze had.
using Microsoft.Web.WebView2.Core;
using PowerBIViewerApp;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PowerBIViewer.App.Helpers;

// Namespace moet overeenkomen met x:Class in je XAML.
namespace PowerBIViewer.App.Views
{
    public partial class MainWindow : Window
    {
        private bool isDarkMode = false;
        private string _currentReportName = string.Empty;

        public MainWindow()
        {
            InitializeComponent();
            DwmApiHelper.SetTitleBarTheme(this, isDarkMode);
            // Koppel alle events
            PowerBIWebView.NavigationStarting += PowerBIWebView_NavigationStarting;
            PowerBIWebView.NavigationCompleted += PowerBIWebView_NavigationCompleted;
            this.Closing += MainWindow_Closing;

            this.KeyDown += MainWindow_KeyDown;

            // Laad de vensterstatus voordat de rest wordt geladen
            RestoreWindowState();

            // --- TOEGEVOEGD: Stel de themaknop correct in bij opstarten ---
            UpdateThemeButtonContent();

            // Overige opstartlogica
            CreateDashboardButtons();
            LoadDashboardByKey("sales");
        }

        // --- Vensterstatus Beheer ---
        private void ToggleFullScreen()
        {
            // Controleer de huidige WindowState
            if (this.WindowState == WindowState.Maximized && this.WindowStyle == WindowStyle.None)
            {
                // We zijn in full-screen modus, dus ga terug naar normaal
                this.WindowState = WindowState.Normal;
                this.WindowStyle = WindowStyle.SingleBorderWindow;
                this.ResizeMode = ResizeMode.CanResize;

                // Verander het icoon en de tooltip van de knop
                FullScreenButton.Content = "⛶";
                FullScreenButton.ToolTip = "Volledig scherm (F11)";
            }
            else
            {
                // We zijn niet in full-screen, dus ga ernaartoe
                this.WindowState = WindowState.Maximized;
                this.WindowStyle = WindowStyle.None;
                this.ResizeMode = ResizeMode.NoResize;

                // Verander het icoon en de tooltip van de knop
                FullScreenButton.Content = "⤡"; // 'Inkrimpen' icoon
                FullScreenButton.ToolTip = "Verlaat volledig scherm (F11)";
            }
        }
        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            // Reageer op het indrukken van de F11-toets
            if (e.Key == Key.F11)
            {
                ToggleFullScreen();
            }
        }

        private void ToggleFullScreen_Click(object sender, RoutedEventArgs e)
        {
            // Reageer op de klik op de knop
            ToggleFullScreen();
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

        private void MainWindow_Closing(object sender, CancelEventArgs e)


        {
            this.WindowStyle = WindowStyle.SingleBorderWindow;
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

        // --- Laadlogica ---

        private void LoadDashboardByKey(string key)
        {
            var report = ReportRepository.GetByKey(key);
            if (report != null)
            {
                // --- NIEUW: Onthoud de naam van het rapport dat we gaan laden ---
                _currentReportName = report.Name;
                LoadDashboard(report.Url);
            }
            else
            {
                MessageBox.Show($"Dashboard met sleutel '{key}' niet gevonden.", "Fout", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void LoadDashboard(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                PowerBIWebView.Source = new Uri(url);
            }
            else
            {
                MessageBox.Show("Geen geldige URL gevonden voor dit dashboard.");
            }
        }

        // --- WebView2 Events voor de Laad-indicator ---

        private void PowerBIWebView_NavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs e)
        {
            // Toon de indicator
            LoadingIndicator.Visibility = Visibility.Visible;
            PowerBIWebView.Visibility = Visibility.Collapsed;

            // --- NIEUW: Update de statusbalk ---
            StatusTextBlock.Text = $"Rapport '{_currentReportName}' wordt geladen...";
        }
        private void PowerBIWebView_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            // Verberg de indicator
            LoadingIndicator.Visibility = Visibility.Collapsed;
            PowerBIWebView.Visibility = Visibility.Visible;

            if (e.IsSuccess)
            {
                // --- NIEUW: Update de statusbalk met een succesbericht ---
                StatusTextBlock.Text = $"Rapport '{_currentReportName}' succesvol geladen.";
            }
            else
            {
                // --- NIEUW: Update de statusbalk met een foutbericht ---
                StatusTextBlock.Text = $"Fout bij het laden van '{_currentReportName}'.";
                MessageBox.Show($"Kon de pagina niet laden. Foutcode: {e.WebErrorStatus}", "Navigatiefout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // --- Thema Beheer ---

        private void UpdateThemeButtonContent()
        {
            if (isDarkMode)
            {
                ThemeToggleButton.Content = "☀️";
                ThemeToggleButton.ToolTip = "Wissel naar Licht thema";
            }
            else
            {
                ThemeToggleButton.Content = "🌙";
                ThemeToggleButton.ToolTip = "Wissel naar Donker thema";
            }
        }

        private void ToggleTheme_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 1. Wissel de status
                isDarkMode = !isDarkMode;

                DwmApiHelper.SetTitleBarTheme(this, isDarkMode);
                // 2. Laad de nieuwe themabestanden
                string newThemeFileName = isDarkMode ? "Themes/DarkMode.xaml" : "Themes/LightMode.xaml";
                Uri newThemeUri = new Uri(newThemeFileName, UriKind.Relative);

                var existingTheme = Application.Current.Resources.MergedDictionaries
                    .FirstOrDefault(d => d.Source != null && d.Source.OriginalString.Contains("Mode.xaml"));

                if (existingTheme != null)
                {
                    Application.Current.Resources.MergedDictionaries.Remove(existingTheme);
                }

                var newTheme = new ResourceDictionary { Source = newThemeUri };
                Application.Current.Resources.MergedDictionaries.Add(newTheme);

                // --- TOEGEVOEGD: Update de knopinhoud na de wissel ---
                UpdateThemeButtonContent();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij het wisselen van thema:\n{ex.Message}", "Thema Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // --- Actieknoppen ---

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            if (PowerBIWebView != null && PowerBIWebView.CoreWebView2 != null)
            {
                PowerBIWebView.Reload();
            }
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            var aboutWindow = new AboutWindow { Owner = this };
            aboutWindow.ShowDialog();
        }

        private void OpenWidgetLauncher_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WidgetLauncher launcher = new WidgetLauncher();
                launcher.Owner = this;
                launcher.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kon de Widget Launcher niet openen:\n{ex.Message}", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Screenshot_Click(object sender, RoutedEventArgs e) { MessageBox.Show("Screenshot-functie wordt later toegevoegd."); }
        private void OpenSettings_Click(object sender, RoutedEventArgs e) { MessageBox.Show("Instellingenvenster wordt later toegevoegd."); }

        // --- Dynamische UI ---

        private void CreateDashboardButtons()
        {
            // Deze methode voegt nu knoppen TOE aan de bestaande vaste knoppen.
            foreach (var report in ReportRepository.GetAll())
            {
                var button = new Button
                {
                    Content = $"{report.Emoji} {report.Name}",
                    Margin = new Thickness(4),
                    Padding = new Thickness(10, 5, 10, 5),
                    Tag = report.Key,
                    Style = (Style)FindResource("SecondaryButton")
                };
                button.Click += (s, e) => { LoadDashboardByKey((string)((Button)s).Tag); };
                DashboardButtonPanel.Children.Add(button);
            }
        }
        private void CommunityButton_Click(object sender, RoutedEventArgs e)
        {
            // URL van de Power BI Data Stories Gallery
            const string communityUrl = "https://community.powerbi.com/t5/Data-Stories-Gallery/bd-p/DataStoriesGallery";

            // Update de statusbalk en laad het dashboard
            _currentReportName = "Power BI Community Gallery";
            LoadDashboard(communityUrl);
        }

        private void NovyProButton_Click(object sender, RoutedEventArgs e)
        {
            // URL van de NovyPro explore pagina
            const string novyProUrl = "https://www.novypro.com/explore_projects";

            // Update de statusbalk en laad het dashboard
            _currentReportName = "NovyPro Explore";
            LoadDashboard(novyProUrl);
        }
    }
}