using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace Pagamentos.Utils
{
    public sealed class CurrencyFormatConverter : IValueConverter
    {
        private static readonly CultureInfo PtBrCulture = new("pt-BR");

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
            {
                return FormatCurrency(0m);
            }

            if (value is decimal decimalValue)
            {
                return FormatCurrency(decimalValue);
            }

            if (value is double doubleValue)
            {
                return FormatCurrency((decimal)doubleValue);
            }

            var stringValue = value.ToString();
            if (string.IsNullOrWhiteSpace(stringValue))
            {
                return FormatCurrency(0m);
            }

            if (decimal.TryParse(stringValue, NumberStyles.Any, PtBrCulture, out var parsedPtBr))
            {
                return FormatCurrency(parsedPtBr);
            }

            if (decimal.TryParse(stringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsedInvariant))
            {
                return FormatCurrency(parsedInvariant);
            }

            return stringValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue)
            {
                var cleanedValue = stringValue.Replace("R$", string.Empty, StringComparison.OrdinalIgnoreCase).Trim();

                if (decimal.TryParse(cleanedValue, NumberStyles.Any, PtBrCulture, out var parsed))
                {
                    return parsed.ToString("0.00", CultureInfo.InvariantCulture);
                }
            }

            return value;
        }

        private static string FormatCurrency(decimal value) => value.ToString("C", PtBrCulture);
    }
}
