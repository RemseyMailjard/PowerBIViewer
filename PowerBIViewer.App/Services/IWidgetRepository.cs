// FILE: Services/IWidgetRepository.cs
using PowerBIViewer.App.Models;
using System.Collections.Generic;

namespace PowerBIViewer.App.Services
{
    public interface IWidgetRepository
    {
        List<Widget> LoadWidgets();
        void SaveWidgets(List<Widget> widgets);
    }
}