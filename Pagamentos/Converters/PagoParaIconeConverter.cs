using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace Pagamentos.Converters
{
    public class PagoParaIconeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isPaid = (bool)value;
            return isPaid ? "check.png" : "unpaid.png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
