using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace Pagamentos.Utils
{
    public class PagoImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isPaid)
            {
                return isPaid ? "check.png" : "unpaid.png";
            }

            return "unpaid.png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
