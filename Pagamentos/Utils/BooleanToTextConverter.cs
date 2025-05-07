using System.Globalization;


namespace Pagamentos.Utils
{
    public class BooleanToTextConverter : IValueConverter
    {
        public string TrueText { get; set; } = "Sim";
        public string FalseText { get; set; } = "Não";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool booleanValue)
            {
                return booleanValue ? TrueText : FalseText;
            }

            return FalseText;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

