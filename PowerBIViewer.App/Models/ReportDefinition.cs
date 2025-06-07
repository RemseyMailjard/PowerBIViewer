// FILE: Models/ReportDefinition.cs
using PowerBIViewer.App.ViewModels; // Nodig voor ViewModelBase
using System.Text.Json.Serialization;

namespace PowerBIViewer.App.Models // ✨ Aangepaste namespace
{
    // ✨ GEWIJZIGD: Erft nu van ViewModelBase
    public class ReportDefinition : ViewModelBase
    {
        public string Name { get; set; }
        public string Key { get; set; }
        public string Url { get; set; }
        public string Emoji { get; set; }

        private bool _isFavorite;

        // ✨ NIEUW: Een property voor de favorietenstatus.
        // [JsonIgnore] zorgt ervoor dat deze property niet wordt meegenomen
        // bij het serialiseren naar reports.json.
        [JsonIgnore]
        public bool IsFavorite
        {
            get => _isFavorite;
            set { _isFavorite = value; OnPropertyChanged(); }
        }
    }
}