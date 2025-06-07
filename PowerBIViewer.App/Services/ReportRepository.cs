// FILE: Services/ReportRepository.cs
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace PowerBIViewer.App
{
    // De ReportDefinition class blijft ongewijzigd
    public class ReportDefinition
    {
        public string Name { get; set; }
        public string Key { get; set; }
        public string Url { get; set; }
        public string Emoji { get; set; }
    }

    // ✨ VERBETERINGEN:
    // 1. De class is niet langer 'static'.
    // 2. De class implementeert de 'IReportRepository' interface.
    public class ReportRepository : IReportRepository
    {
        private readonly List<ReportDefinition> _reports;

        // De constructor (niet-statisch) wordt nu aangeroepen wanneer de DI container
        // een nieuwe instantie van deze class aanmaakt.
        public ReportRepository()
        {
            try
            {
                // Let op: Dit pad is relatief aan de uitvoeringsmap (bv. bin/Debug/...)
                string json = File.ReadAllText("Data/reports.json");
                _reports = JsonSerializer.Deserialize<List<ReportDefinition>>(json) ?? new List<ReportDefinition>();
            }
            catch (System.Exception)
            {
                // Fallback als het bestand niet bestaat of corrupt is.
                _reports = new List<ReportDefinition>();
            }
        }

        // Deze methodes zijn nu instance-methods, geen static methods meer.
        public List<ReportDefinition> GetAll() => _reports;

        public ReportDefinition? GetByKey(string key) =>
            _reports.FirstOrDefault(r => r.Key == key);
    }
}