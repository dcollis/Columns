using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace Columns
{
    public class IntToColourConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                int boxType = (int)value;

                switch (boxType)
                {
                    case 0: return new SolidColorBrush(Colors.Red);
                    case 1: return new SolidColorBrush(Colors.Green);
                    case 2: return new SolidColorBrush(Colors.Yellow);
                    case 3: return new SolidColorBrush(Colors.Blue);
                    case 4: return new SolidColorBrush(Colors.Black);
                    case 5: return new SolidColorBrush(Colors.White);
                    case 6: return new SolidColorBrush(Colors.Orange);
                    case 7: return new SolidColorBrush(Colors.Purple);
                    case 8: return new SolidColorBrush(Colors.Brown);
                    case 9: return new SolidColorBrush(Colors.Gray);
                    
                    default: break;
                }
            }
            return null;
        }
        
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) 
        {
            return null;
        }
    }


}
