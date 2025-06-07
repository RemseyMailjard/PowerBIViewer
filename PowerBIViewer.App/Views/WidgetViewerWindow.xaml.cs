using Microsoft.Web.WebView2.Core;
using System;
using System.Windows;

namespace PowerBIViewerApp
{
    public partial class WidgetViewerWindow : Window
    {
        private readonly string _widgetUrl;

        public WidgetViewerWindow(string widgetUrl)
        {
            InitializeComponent(); // 🔧 Zorgt dat de XAML gekoppeld wordt aan deze class
            _widgetUrl = widgetUrl;
        }

        private async void WidgetViewerWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                await WidgetWebView.EnsureCoreWebView2Async();

                WidgetWebView.CoreWebView2.NavigationCompleted += WebView_NavigationCompleted;
                WidgetWebView.Source = new Uri(_widgetUrl);

                WidgetWebView.CoreWebView2.Settings.IsStatusBarEnabled = false;
                WidgetWebView.CoreWebView2.Settings.IsZoomControlEnabled = false;
                WidgetWebView.CoreWebView2.Settings.AreDevToolsEnabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij laden WebView2:\n{ex.Message}", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void WebView_NavigationCompleted(object? sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            string js = @"
(() => {
    // Probeer iframe te vinden
    const iframe = document.querySelector('iframe');
    if (iframe) {
        iframe.style.width = '100vw';
        iframe.style.height = '100vh';
        iframe.style.border = 'none';

        // Zoek mogelijke containers en zet overflow/visibility uit
        const parent = iframe.parentElement;
        if (parent) {
            parent.style.margin = '0';
            parent.style.padding = '0';
            parent.style.overflow = 'hidden';
        }

        // Simuleer fullscreen: viewport-resize
        setTimeout(() => {
            window.dispatchEvent(new Event('resize'));
            document.body.style.margin = '0';
            document.body.style.padding = '0';
            document.body.style.overflow = 'hidden';
        }, 600);

        // Fallback: probeer nogmaals na 2 seconden
        setTimeout(() => {
            iframe.style.height = '100vh';
            window.dispatchEvent(new Event('resize'));
        }, 2000);
    }
})();
";
            try
            {
                await WidgetWebView.CoreWebView2.ExecuteScriptAsync(js);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"JavaScript-injectie mislukt:\n{ex.Message}", "Fout", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
