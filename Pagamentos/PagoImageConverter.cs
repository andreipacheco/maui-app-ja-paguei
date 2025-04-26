using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace Pagamentos
{
    public class PagoImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isPaid)
            {
                return isPaid ? "check.png" : "unpaid.png";
                // "check.png" = pago
                // "unpaid.png" = não pago
                // (Você precisa ter essas imagens no seu projeto, posso te ajudar a criar se quiser)
            }

            return "unpaid.png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
