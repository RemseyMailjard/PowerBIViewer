// FILE: PowerBIViewer.App/Services/ReportRepository.cs
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace PowerBIViewer.App
{
    // Dit blijft hetzelfde
    public class ReportDefinition
    {
        public string Name { get; set; }
        public string Key { get; set; }
        public string Url { get; set; }
        public string Emoji { get; set; }
    }

    // De class is niet langer 'static'
    public static class ReportRepository
    {
        private static List<ReportDefinition> _reports;

        // De static constructor laadt de data één keer bij de eerste aanroep.
        static ReportRepository()
        {
            try
            {
                string json = File.ReadAllText("Data/reports.json");
                _reports = JsonSerializer.Deserialize<List<ReportDefinition>>(json) ?? new List<ReportDefinition>();
            }
            catch (System.Exception)
            {
                // Fallback als het bestand niet bestaat of corrupt is
                _reports = new List<ReportDefinition>();
            }
        }

        public static List<ReportDefinition> GetAll() => _reports;

        public static ReportDefinition? GetByKey(string key) =>
            _reports.FirstOrDefault(r => r.Key == key);
    }
}