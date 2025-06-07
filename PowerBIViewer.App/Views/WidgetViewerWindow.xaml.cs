// WidgetViewerWindow.xaml.cs
using System;
using System.Windows;

namespace PowerBIViewerApp
{
    public partial class WidgetViewerWindow : Window
    {
        public WidgetViewerWindow(string widgetUrl)
        {
            InitializeComponent();

            if (!string.IsNullOrEmpty(widgetUrl))
            {
                try
                {
                    WidgetWebView.Source = new Uri(widgetUrl);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Kan de widget niet laden:\n{ex.Message}", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
