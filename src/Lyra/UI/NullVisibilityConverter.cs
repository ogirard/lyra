using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Lyra.UI
{
    public class NullVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var visibility = (Visibility?)parameter ?? Visibility.Hidden;

            if (value == null)
            {
                return visibility;
            }

            return visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}