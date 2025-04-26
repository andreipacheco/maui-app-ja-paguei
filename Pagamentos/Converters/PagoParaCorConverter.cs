using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace Pagamentos.Converters
{
    public class PagoParaCorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isPaid = (bool)value;
            return isPaid ? Colors.Green : Colors.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
