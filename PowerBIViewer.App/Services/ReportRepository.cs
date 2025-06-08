// FILE: Services/ReportRepository.cs
using PowerBIViewer.App.Models; // Zorg dat deze using er staat!
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace PowerBIViewer.App.Services // BELANGRIJK: Moet exact deze namespace zijn.
{
    public class ReportRepository : IReportRepository
    {
        private readonly List<ReportDefinition> _reports;
        private const string FilePath = "Data/reports.json";

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

        public List<ReportDefinition> GetAll() => _reports;

        public ReportDefinition? GetByKey(string key) =>
            _reports.FirstOrDefault(r => r.Key == key);

        public void SaveReports(List<ReportDefinition> reports)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(reports, options);
            File.WriteAllText(FilePath, json);
        }
    }
}