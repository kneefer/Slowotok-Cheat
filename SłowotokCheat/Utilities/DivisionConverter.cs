using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SłowotokCheat.Utilities
{
    class DivisionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double doubleValue, doubleParameter;


            if (!Double.TryParse(value.ToString(), out doubleValue))
            {
                throw new FormatException();
            }

            if (Double.TryParse(parameter.ToString(), out doubleParameter))
            {
                return doubleParameter * doubleValue / 100;
            }

            throw new FormatException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
