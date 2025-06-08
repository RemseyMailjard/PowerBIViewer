// FILE: Views/WidgetLauncher.xaml.cs
using Microsoft.Extensions.DependencyInjection;
using PowerBIViewer.App;
using PowerBIViewer.App.Helpers;
using PowerBIViewer.App.ViewModels;
using PowerBIViewer.App.Views;
using System; // ✨ TOEGEVOEGD
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace PowerBIViewerApp
{
    public partial class WidgetLauncher : Window
    {
        // ✨ NIEUW: Variabele om de huidige themastatus bij te houden.
        private bool _isDarkMode;

        public WidgetLauncher(WidgetLauncherViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;

            // Bepaal de initiële modus en pas de titelbalk en knop aan.
            _isDarkMode = IsDarkModeActive();
            DwmApiHelper.SetTitleBarTheme(this, _isDarkMode);
            UpdateThemeButtonContent();
        }

        private void WidgetListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is WidgetLauncherViewModel vm && ListBox.SelectedItem != null)
            {
                if (vm.OpenOnDoubleClickCommand.CanExecute(ListBox.SelectedItem))
                {
                    vm.OpenOnDoubleClickCommand.Execute(ListBox.SelectedItem);
                }
            }
        }

        private void OpenSettings_Click(object sender, RoutedEventArgs e)
        {
            var settingsWindow = App.ServiceProvider?.GetService<SettingsWindow>();
            if (settingsWindow != null)
            {
                settingsWindow.Owner = this;
                settingsWindow.ShowDialog();
            }
        }

        // ✨ NIEUW: De volledige logica voor het wisselen van thema.
        private void ToggleTheme_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _isDarkMode = !_isDarkMode;

                // Pas titelbalk aan
                DwmApiHelper.SetTitleBarTheme(this, _isDarkMode);

                // Verwijder oud thema uit de applicatie-resources
                var existingTheme = Application.Current.Resources.MergedDictionaries
                    .FirstOrDefault(d => d.Source != null && d.Source.OriginalString.Contains("Mode.xaml"));
                if (existingTheme != null)
                {
                    Application.Current.Resources.MergedDictionaries.Remove(existingTheme);
                }

                // Voeg nieuw thema toe
                string newThemeFileName = _isDarkMode ? "Themes/DarkMode.xaml" : "Themes/LightMode.xaml";
                var newTheme = new ResourceDictionary { Source = new Uri(newThemeFileName, UriKind.Relative) };
                Application.Current.Resources.MergedDictionaries.Add(newTheme);

                // Update de knop-icoon/tooltip
                UpdateThemeButtonContent();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij het wisselen van thema:\n{ex.Message}", "Thema Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ✨ NIEUW: Helper-methode om de knop-content bij te werken.
        private void UpdateThemeButtonContent()
        {
            if (_isDarkMode)
            {
                ThemeToggleButton.Content = "☀️";
                ThemeToggleButton.ToolTip = "Wissel naar Licht thema";
            }
            else
            {
                ThemeToggleButton.Content = "🌓";
                ThemeToggleButton.ToolTip = "Wissel naar Donker thema";
            }
        }

        // ✨ NIEUW: Helper-methode om de actieve modus te detecteren.
        private bool IsDarkModeActive()
        {
            return Application.Current.Resources.MergedDictionaries
                .Any(d => d.Source != null && d.Source.OriginalString.Contains("DarkMode.xaml"));
        }
    }
}