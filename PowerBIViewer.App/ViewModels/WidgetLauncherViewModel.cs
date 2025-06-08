// FILE: ViewModels/WidgetLauncherViewModel.cs
using PowerBIViewer.App.Commands;
using PowerBIViewer.App.Models;
using PowerBIViewer.App.Services;
using PowerBIViewerApp; // Voor WidgetViewerWindow
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace PowerBIViewer.App.ViewModels
{
    public class WidgetLauncherViewModel : ViewModelBase
    {
        // --- Properties voor DataBinding ---
        public ObservableCollection<Widget> Widgets { get; }

        private Widget? _selectedWidget;
        public Widget? SelectedWidget
        {
            get => _selectedWidget;
            set { _selectedWidget = value; OnPropertyChanged(); (OpenWidgetCommand as RelayCommand)?.RaiseCanExecuteChanged(); }
        }

        private string _customUrl = string.Empty;
        public string CustomUrl
        {
            get => _customUrl;
            set { _customUrl = value; OnPropertyChanged(); (OpenWidgetCommand as RelayCommand)?.RaiseCanExecuteChanged(); }
        }

        // --- Commands ---
        public ICommand OpenWidgetCommand { get; }
        public ICommand OpenOnDoubleClickCommand { get; }

        public WidgetLauncherViewModel(IWidgetRepository widgetRepository)
        {
            Widgets = new ObservableCollection<Widget>(widgetRepository.LoadWidgets());

            OpenWidgetCommand = new RelayCommand(ExecuteOpenWidget, CanExecuteOpenWidget);
            OpenOnDoubleClickCommand = new RelayCommand(ExecuteOpenOnDoubleClick);
        }

        private void ExecuteOpenOnDoubleClick(object? parameter)
        {
            if (parameter is Widget widget)
            {
                OpenWidgetWindow(widget.Url);
            }
        }

        private void ExecuteOpenWidget(object? p)
        {
            string? urlToOpen = null;

            if (!string.IsNullOrWhiteSpace(CustomUrl) && Uri.IsWellFormedUriString(CustomUrl, UriKind.Absolute))
            {
                urlToOpen = CustomUrl;
            }
            else if (SelectedWidget != null)
            {
                urlToOpen = SelectedWidget.Url;
            }

            if (!string.IsNullOrEmpty(urlToOpen))
            {
                OpenWidgetWindow(urlToOpen);
            }
        }

        private bool CanExecuteOpenWidget(object? p)
        {
            // De knop is actief als er een geldige custom URL is OF een widget is geselecteerd.
            return (Uri.IsWellFormedUriString(CustomUrl, UriKind.Absolute)) || SelectedWidget != null;
        }

        private void OpenWidgetWindow(string url)
        {
            var viewer = new WidgetViewerWindow(url)
            {
                // ✨ BELANGRIJK: GEEN Owner, en start in het midden van het scherm.
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            viewer.Show();
        }
    }
}