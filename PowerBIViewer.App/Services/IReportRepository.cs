// FILE: Services/IReportRepository.cs
using System.Collections.Generic;

namespace PowerBIViewer.App
{
    public interface IReportRepository
    {
        List<ReportDefinition> GetAll();
        ReportDefinition? GetByKey(string key);
    }
}