// FILE: Views/WidgetViewerWindow.xaml.cs
using Microsoft.Web.WebView2.Core;
using PowerBIViewer.App.Helpers; // ✨ TOEGEVOEGD
using System;
using System.Linq;               // ✨ TOEGEVOEGD
using System.Windows;

namespace PowerBIViewerApp
{
    public partial class WidgetViewerWindow : Window
    {
        private readonly string _widgetUrl;

        public WidgetViewerWindow(string widgetUrl)
        {
            InitializeComponent();
            _widgetUrl = widgetUrl;

            // ✨ NIEUW: Roep de methode aan om de titelbalk correct in te stellen
            // zodra het venster wordt aangemaakt.
            ApplyTitleBarTheme();
        }

        // ✨ NIEUW: Helper-methode om het thema van de titelbalk te bepalen en toe te passen.
        private void ApplyTitleBarTheme()
        {
            // Zoek in de applicatie-resources of het DarkMode.xaml bestand is geladen.
            bool isDarkMode = Application.Current.Resources.MergedDictionaries
                .Any(d => d.Source != null && d.Source.OriginalString.Contains("DarkMode.xaml"));

            // Gebruik de DwmApiHelper om de kleur van de titelbalk aan te passen.
            DwmApiHelper.SetTitleBarTheme(this, isDarkMode);
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

        // ✨ GEWIJZIGD: 'sender' is nu 'object?' om nullable-waarschuwingen te voorkomen.
        private async void WebView_NavigationCompleted(object? sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            string js = @"
            (() => {
                const iframe = document.querySelector('iframe');
                if (iframe) {
                    iframe.style.width = '100vw';
                    iframe.style.height = '100vh';
                    iframe.style.border = 'none';
                    const parent = iframe.parentElement;
                    if (parent) {
                        parent.style.margin = '0';
                        parent.style.padding = '0';
                        parent.style.overflow = 'hidden';
                    }
                    setTimeout(() => {
                        window.dispatchEvent(new Event('resize'));
                        document.body.style.margin = '0';
                        document.body.style.padding = '0';
                        document.body.style.overflow = 'hidden';
                    }, 600);
                    setTimeout(() => {
                        iframe.style.height = '100vh';
                        window.dispatchEvent(new Event('resize'));
                    }, 2000);
                }
            })();";
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