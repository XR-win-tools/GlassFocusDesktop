using System;
using System.Globalization;
using System.Windows.Data;

namespace GlassFocusDesktop;

public class BoolToStartPauseConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value is bool b && b ? "暂停" : "开始";

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}