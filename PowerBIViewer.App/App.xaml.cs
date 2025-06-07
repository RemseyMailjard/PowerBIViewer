// FILE: PowerBIViewer.App/App.xaml.cs
using PowerBIViewer.App.Views; // Zorg dat deze using er staat!
using System.Windows;

namespace PowerBIViewer.App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // Belangrijk om de basisfunctionaliteit van de Application class te behouden
            base.OnStartup(e);

            // Maak een nieuwe instantie van je hoofdvenster aan
            MainWindow mainWindow = new MainWindow();

            // Toon het venster
            // De applicatie blijft draaien totdat dit venster gesloten wordt.
            mainWindow.Show();
        }
    }
}