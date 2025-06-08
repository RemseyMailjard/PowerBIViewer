// FILE: Views/SettingsWindow.xaml.cs
using PowerBIViewer.App.ViewModels;
using System.Windows;

namespace PowerBIViewer.App.Views
{
    /// <summary>
    /// De code-behind voor het SettingsWindow.
    /// De enige verantwoordelijkheid is het initialiseren van het component
    /// en het koppelen van de via DI geïnjecteerde ViewModel aan de DataContext.
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow(SettingsViewModel viewModel)
        {
            InitializeComponent();

            // Stel de ontvangen ViewModel in als de DataContext voor de hele View.
            this.DataContext = viewModel;

            // Luister naar het 'OnSaveComplete' event van de ViewModel.
            // Als de ViewModel dit event afvuurt (na succesvol opslaan), sluiten we dit venster.
            // De if-statement is verwijderd omdat dit de correcte manier is om op een event te abonneren.
            viewModel.OnSaveComplete += () => this.Close();
        }
    }
}