using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace Pagamentos.Utils
{
    public class CurrencyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue && decimal.TryParse(stringValue, out decimal decimalValue))
            {
                return decimalValue.ToString("C2", new CultureInfo("pt-BR")); // Formata como moeda brasileira
            }

            return "R$ 0,00"; // Valor padrão
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue)
            {
                // Remove o prefixo "R$" e converte de volta para string
                return stringValue.Replace("R$", "").Trim();
            }

            return "0,00"; // Valor padrão
        }
    }
}
