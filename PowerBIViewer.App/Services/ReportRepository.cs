// FILE: Services/ReportRepository.cs
using System.Collections.Generic;
using System.IO; // ✨ GECORRIGEERD: Nodig voor File.WriteAllText
using System.Linq;
using System.Text.Json;

namespace PowerBIViewer.App.Services // ✨ GECORRIGEERD: Namespace toegevoegd
{
    // De ReportDefinition class hoort hier, bij de implementatie.
    public class ReportDefinition
    {
        public string Name { get; set; }
        public string Key { get; set; }
        public string Url { get; set; }
        public string Emoji { get; set; }
    }

    public class ReportRepository : IReportRepository
    {
        private readonly List<ReportDefinition> _reports;
        private const string FilePath = "Data/reports.json"; // ✨ VERBETERING: Pad in een constante

        public ReportRepository()
        {
            try
            {
                string json = File.ReadAllText(FilePath);
                _reports = JsonSerializer.Deserialize<List<ReportDefinition>>(json) ?? new List<ReportDefinition>();
            }
            catch (System.Exception)
            {
                _reports = new List<ReportDefinition>();
            }
        }

        // De bestaande methodes die de interface implementeren
        public List<ReportDefinition> GetAll() => _reports;

        public ReportDefinition? GetByKey(string key) =>
            _reports.FirstOrDefault(r => r.Key == key);

        // ✨ De nieuwe save-methode die de interface implementeert
        public void SaveReports(List<ReportDefinition> reports)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(reports, options);
            File.WriteAllText(FilePath, json);
        }
    }
}