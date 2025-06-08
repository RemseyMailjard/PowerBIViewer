// FILE: Models/Widget.cs
using PowerBIViewer.App.ViewModels; // ✨ TOEGEVOEGD
using System.ComponentModel;          // ✨ TOEGEVOEGD

namespace PowerBIViewer.App.Models
{
    // ✨ GEWIJZIGD: Erft nu van ViewModelBase en implementeert IDataErrorInfo
    public class Widget : ViewModelBase, IDataErrorInfo
    {
        // --- Properties met backing fields voor PropertyChanged ---
        private string _title = string.Empty;
        public string Title
        {
            get => _title;
            set { _title = value; OnPropertyChanged(); }
        }

        private string _icon = string.Empty;
        public string Icon
        {
            get => _icon;
            set { _icon = value; OnPropertyChanged(); }
        }

        private string _url = string.Empty;
        public string Url
        {
            get => _url;
            set { _url = value; OnPropertyChanged(); }
        }

        public override string ToString()
        {
            return $"{Icon} {Title}";
        }

        // --- IDataErrorInfo Implementatie ---

        public string Error => string.Empty;

        public string this[string columnName]
        {
            get
            {
                string result = string.Empty;
                switch (columnName)
                {
                    case nameof(Title):
                        if (string.IsNullOrWhiteSpace(Title))
                            result = "Titel mag niet leeg zijn.";
                        break;

                    case nameof(Url):
                        if (string.IsNullOrWhiteSpace(Url))
                            result = "URL mag niet leeg zijn.";
                        else if (!System.Uri.IsWellFormedUriString(Url, System.UriKind.Absolute))
                            result = "Voer een geldige, absolute URL in (bv. https://...).";
                        break;
                }
                return result;
            }
        }

        /// <summary>
        /// Helper-property die we in de ViewModel kunnen gebruiken om te
        /// controleren of het hele object geldig is.
        /// </summary>
        public bool IsValid =>
            string.IsNullOrEmpty(this[nameof(Title)]) &&
            string.IsNullOrEmpty(this[nameof(Url)]);
    }
}