// FILE: Services/ReportRepository.cs
using PowerBIViewer.App.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace PowerBIViewer.App.Services
{
    public class ReportRepository : IReportRepository
    {
        private readonly List<ReportDefinition> _reports;
        private const string FilePath = "Data/reports.json";

        // ✨ VERBETERING (voor CA1869): Maak één instantie van de options en hergebruik deze.
        private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
        {
            WriteIndented = true
        };

        public ReportRepository()
        {
            try
            {
                string json = File.ReadAllText(FilePath);
                // De `?? new()` zorgt voor een fallback als deserialisatie 'null' retourneert.
                _reports = JsonSerializer.Deserialize<List<ReportDefinition>>(json) ?? [];
            }
            catch (System.Exception)
            {
                // ✨ VERBETERING (voor IDE0028): Gebruik de versimpelde 'new()' syntax.
                _reports = [];
            }
        }

        public List<ReportDefinition> GetAll() => _reports;

        public ReportDefinition? GetByKey(string key) =>
            _reports.FirstOrDefault(r => r.Key == key);

        public void SaveReports(List<ReportDefinition> reports)
        {
            // ✨ VERBETERING (voor CA1869): Gebruik het gecachte options-object.
            string json = JsonSerializer.Serialize(reports, _jsonSerializerOptions);
            File.WriteAllText(FilePath, json);
        }
    }
}