// FILE: PowerBIViewer.App/Converters/EqualityConverter.cs
using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace PowerBIViewer.App.Converters
{
    public class EqualityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // Controleer of we twee waarden hebben en of ze niet null zijn
            if (values == null || values.Length < 2 || values.Any(v => v == null))
            {
                return false;
            }

            // Vergelijk de twee waarden met elkaar
            return values[0].Equals(values[1]);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            // ConvertBack is niet nodig voor deze toepassing
            throw new NotImplementedException();
        }
    }
}