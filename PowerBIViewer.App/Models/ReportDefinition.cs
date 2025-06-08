// FILE: Models/ReportDefinition.cs
using PowerBIViewer.App.ViewModels;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace PowerBIViewer.App.Models
{
    /// <summary>
    /// Representeert een enkel Power BI rapport in de applicatie.
    /// Erft van ViewModelBase om PropertyChanged notificaties te ondersteunen voor UI updates.
    /// Implementeert IDataErrorInfo voor input validatie in de UI.
    /// </summary>
    public class ReportDefinition : ViewModelBase, IDataErrorInfo
    {
        private string _name = string.Empty;
        /// <summary>
        /// De gebruiksvriendelijke naam van het rapport, zoals getoond in de UI.
        /// </summary>
        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        private string _key = string.Empty;
        /// <summary>
        /// Een unieke, programmatische identifier voor het rapport.
        /// </summary>
        public string Key
        {
            get => _key;
            set { _key = value; OnPropertyChanged(); }
        }

        private string _url = string.Empty;
        /// <summary>
        /// De volledige URL naar het Power BI rapport (embed link).
        /// </summary>
        public string Url
        {
            get => _url;
            set { _url = value; OnPropertyChanged(); }
        }

        private string _emoji = string.Empty;
        /// <summary>
        /// Een optioneel emoji-icoon om het rapport visueel te identificeren.
        /// </summary>
        public string Emoji
        {
            get => _emoji;
            set { _emoji = value; OnPropertyChanged(); }
        }

        private bool _isFavorite;
        /// <summary>
        /// Geeft aan of de gebruiker dit rapport als favoriet heeft gemarkeerd.
        /// Deze eigenschap wordt niet opgeslagen in reports.json.
        /// </summary>
        [JsonIgnore]
        public bool IsFavorite
        {
            get => _isFavorite;
            set { _isFavorite = value; OnPropertyChanged(); }
        }

        // --- IDataErrorInfo Implementatie ---

        /// <summary>
        /// Niet gebruikt in deze implementatie, maar vereist door de interface.
        /// </summary>
        [JsonIgnore]
        public string Error => string.Empty;

        /// <summary>
        /// Wordt aangeroepen door WPF bindings om validatiefouten per property op te vragen.
        /// </summary>
        /// <param name="columnName">De naam van de property die gevalideerd wordt.</param>
        /// <returns>Een foutmelding als de waarde ongeldig is, anders een lege string.</returns>
        public string this[string columnName]
        {
            get
            {
                string result = string.Empty;
                switch (columnName)
                {
                    case nameof(Name):
                        if (string.IsNullOrWhiteSpace(Name))
                            result = "Naam mag niet leeg zijn.";
                        break;

                    case nameof(Key):
                        if (string.IsNullOrWhiteSpace(Key))
                            result = "Key mag niet leeg zijn.";
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
        /// Een helper-property die true teruggeeft als alle validatieregels voor dit object slagen.
        /// </summary>
        [JsonIgnore]
        public bool IsValid =>
            string.IsNullOrEmpty(this[nameof(Name)]) &&
            string.IsNullOrEmpty(this[nameof(Key)]) &&
            string.IsNullOrEmpty(this[nameof(Url)]);
    }
}