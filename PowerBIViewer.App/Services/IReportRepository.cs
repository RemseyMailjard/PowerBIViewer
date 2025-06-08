// FILE: Services/IReportRepository.cs
using System.Collections.Generic;
using PowerBIViewer.App.Models; // Zorg dat deze using er staat als ReportDefinition in Models staat.

namespace PowerBIViewer.App.Services
{
    public interface IReportRepository
    {
        List<ReportDefinition> GetAll();
        ReportDefinition? GetByKey(string key);
        void SaveReports(List<ReportDefinition> reports);
    }
}