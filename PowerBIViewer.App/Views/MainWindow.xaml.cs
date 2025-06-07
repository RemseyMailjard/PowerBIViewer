
namespace PowerBIViewer.App.Views
{
    using PowerBIViewerApp;
    using System;
    using System.Linq; // Voeg deze using toe voor FirstOrDefault
    using System.Windows;
    using System.Windows.Controls;

    // De rest van je code blijft hetzelfde, maar zit nu in de juiste namespace.
    public partial class MainWindow : Window
    {
        private bool isDarkMode = false;

        public MainWindow()
        {
            InitializeComponent(); // Deze werkt nu!
            CreateDashboardButtons();
            LoadDashboardByKey("sales");
        }

        private void LoadDashboard(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                PowerBIWebView.Source = new Uri(url); // Werkt nu
            }
            else
            {
                MessageBox.Show("Geen geldige URL gevonden voor dit dashboard.");
            }
        }

        private void ToggleTheme_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                isDarkMode = !isDarkMode;
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
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij het wisselen van thema:\n{ex.Message}", "Thema Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CreateDashboardButtons()
        {
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
                DashboardButtonPanel.Children.Add(button); // Werkt nu
            }
        }

        private void LoadDashboardByKey(string key)
        {
            var report = ReportRepository.GetByKey(key);
            if (report != null) { LoadDashboard(report.Url); }
            else { MessageBox.Show($"Dashboard met sleutel '{key}' niet gevonden.", "Fout", MessageBoxButton.OK, MessageBoxImage.Warning); }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            if (PowerBIWebView != null && PowerBIWebView.CoreWebView2 != null)
            {
                PowerBIWebView.Reload(); // Werkt nu
            }
        }

        private void Screenshot_Click(object sender, RoutedEventArgs e) { MessageBox.Show("Screenshot-functie wordt later toegevoegd."); }
        private void OpenSettings_Click(object sender, RoutedEventArgs e) { MessageBox.Show("Instellingenvenster wordt later toegevoegd."); }

        private void OpenWidgetLauncher_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WidgetLauncher launcher = new WidgetLauncher();
                launcher.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kon de Widget Launcher niet openen:\n{ex.Message}", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}