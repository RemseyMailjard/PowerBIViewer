using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows;
using PowerBIViewer.App.Models;

namespace PowerBIViewer.App.Services
{
    public class WidgetRepository : IWidgetRepository
    {
        private readonly string _filePath;

        public WidgetRepository(string filePath)
        {
            _filePath = filePath;
        }

        public List<Widget> LoadWidgets()
        {
            try
            {
                // Debug: Toon het pad dat wordt gebruikt
        //        MessageBox.Show($"Laden vanaf:\n{_filePath}", "WidgetRepository Debug");

                if (!File.Exists(_filePath))
                {
                    // Bestand bestaat niet, maak standaard widgets aan
                    var defaultWidgets = new List<Widget>
                    {
                        new Widget { Title = "KPI Dashboard", Icon = "📊", Url = "https://app.powerbi.com/view?r=eyJrIjoiDEMO1" },
                        new Widget { Title = "Rapport Explorer", Icon = "🗂️", Url = "https://app.powerbi.com/view?r=eyJrIjoiDEMO2" },
                        new Widget { Title = "Real-Time Monitor", Icon = "⏱️", Url = "https://app.powerbi.com/view?r=eyJrIjoiDEMO3" }
                    };

                    SaveWidgets(defaultWidgets); // automatisch opslaan
                //    MessageBox.Show("Standaard widgets.json is aangemaakt.", "WidgetRepository");
                    return defaultWidgets;
                }

                var json = File.ReadAllText(_filePath);
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var widgets = JsonSerializer.Deserialize<List<Widget>>(json, options) ?? new List<Widget>();

            //    MessageBox.Show($"Aantal widgets geladen: {widgets.Count}", "WidgetRepository");

                return widgets;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij het laden van widgets:\n{ex.Message}", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                return new List<Widget>();
            }
        }

        public void SaveWidgets(List<Widget> widgets)
        {
            try
            {
                // Zorg dat de map bestaat
                string directory = Path.GetDirectoryName(_filePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var json = JsonSerializer.Serialize(widgets, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_filePath, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij het opslaan van widgets:\n{ex.Message}", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
