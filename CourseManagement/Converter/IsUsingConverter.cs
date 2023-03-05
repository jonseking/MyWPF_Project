using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CourseManagement.Converter
{
    public class IsUsingConverter : IValueConverter
    {
        //从model》》View
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
            {
                return "Collapsed";
            }
            if (value.ToString() == parameter.ToString())
            {
                return "Visible";
            }
            else
            {
                return "Collapsed";
            }
        }

        //从View》》model
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
