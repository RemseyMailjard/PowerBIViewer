using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace PowerBIViewer.App
{
    public class ReportDefinition
    {
        public string Name { get; set; }
        public string Key { get; set; }
        public string Url { get; set; }
        public string Emoji { get; set; }
    }

    public static class ReportRepository
    {
        private static List<ReportDefinition> _reports;
        public static List<ReportDefinition> GetAll() => _reports;


        static ReportRepository()
        {
            LoadReports();
        }

        private static void LoadReports()
        {
            string json = File.ReadAllText("Data/reports.json");
            _reports = JsonSerializer.Deserialize<List<ReportDefinition>>(json);
        }

        public static ReportDefinition GetByKey(string key) =>
            _reports.Find(r => r.Key == key);

      
    }
}
