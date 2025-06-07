// FILE: Services/IReportRepository.cs
using System.Collections.Generic;

namespace PowerBIViewer.App.Services // ✨ GECORRIGEERD: Namespace toegevoegd
{
    public interface IReportRepository
    {
        List<ReportDefinition> GetAll();
        ReportDefinition? GetByKey(string key);
        void SaveReports(List<ReportDefinition> reports);
    }
}