using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace Xam.Plugin.TabView.Converters
{
    class SelectedTabHeaderToTabBackgroundColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isCurrentTabSelected = false;
            if (!string.IsNullOrWhiteSpace(value?.ToString()))
                bool.TryParse(value.ToString(), out isCurrentTabSelected);

            if (parameter is TabViewControl tvc && isCurrentTabSelected)
            {
                return tvc.HeaderSelectionUnderlineColor;
            }
            else
            {
                return Color.Transparent;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}