using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PowerBIViewer.App;
using PowerBIViewer.App.Models;
using PowerBIViewer.App.Services;

namespace PowerBIViewerApp
{
    public partial class WidgetLauncher : Window
    {
        private List<Widget> _widgets;

        public WidgetLauncher()
        {
            InitializeComponent();
            LoadWidgetsFromRepository();
        }

        private void LoadWidgetsFromRepository()
        {
            string jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "widgets.json");
            var repository = new WidgetRepository(jsonPath);
            _widgets = repository.LoadWidgets();
            WidgetListBox.ItemsSource = _widgets;
        }

        private void WidgetListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Check of er echt op een item is geklikt
            if (GetItemUnderMouse(WidgetListBox, e) is Widget selectedWidget)
            {
                OpenWidgetWindow(selectedWidget.Url);
            }
        }

        private void OpenSelectedWidget_Click(object sender, RoutedEventArgs e)
        {
            string widgetUrl = null;

            string customUrl = CustomWidgetUrlTextBox.Text?.Trim();
            if (!string.IsNullOrEmpty(customUrl) && Uri.IsWellFormedUriString(customUrl, UriKind.Absolute))
            {
                widgetUrl = customUrl;
            }
            else if (WidgetListBox.SelectedItem is Widget selectedWidget)
            {
                widgetUrl = selectedWidget.Url;
            }

            if (!string.IsNullOrEmpty(widgetUrl))
            {
                OpenWidgetWindow(widgetUrl);
            }
            else
            {
                MessageBox.Show("Selecteer een widget of voer een geldige URL in.", "Let op", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void OpenWidgetWindow(string url)
        {
            var viewer = new WidgetViewerWindow(url)
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            viewer.Show(); // Laat de launcher gewoon open
        }

        private void WidgetListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (WidgetListBox.SelectedItem != null)
            {
                CustomWidgetUrlTextBox.Text = string.Empty;
            }
        }

        private static object? GetItemUnderMouse(ListBox listBox, MouseButtonEventArgs e)
        {
            var point = e.GetPosition(listBox);
            var element = listBox.InputHitTest(point) as FrameworkElement;
            return element?.DataContext is Widget widget ? widget : null;
        }
    }
}
