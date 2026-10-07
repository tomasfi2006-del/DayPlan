using System;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using Windows.UI.Text;

namespace Planner.Converters;

public class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        bool b = value is bool flag && flag;
        if (parameter is string param && param.Equals("inverse", StringComparison.OrdinalIgnoreCase))
        {
            b = !b;
        }
        return b ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) =>
        value is Visibility visibility && visibility == Visibility.Visible;
}

public class BoolToStrikeThroughConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return value is true ? TextDecorations.Strikethrough : TextDecorations.None;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) =>
        false;
}

public class StringToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        bool hasText = value is string s && !string.IsNullOrWhiteSpace(s);
        return hasText ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) =>
        string.Empty;
}

public class HexToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is string hex && !string.IsNullOrEmpty(hex))
        {
            try
            {
                hex = hex.Replace("#", "");
                byte a = 255;
                int pos = 0;
                if (hex.Length == 8)
                {
                    a = System.Convert.ToByte(hex.Substring(0, 2), 16);
                    pos = 2;
                }
                byte r = System.Convert.ToByte(hex.Substring(pos, 2), 16);
                byte g = System.Convert.ToByte(hex.Substring(pos + 2, 2), 16);
                byte b = System.Convert.ToByte(hex.Substring(pos + 4, 2), 16);
                return new SolidColorBrush(Windows.UI.Color.FromArgb(a, r, g, b));
            }
            catch
            {
                // Fallback
            }
        }
        return new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 95, 184));
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) =>
        "#005FB8";
}

public class BoolToGridLengthConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        double width = 250.0;
        if (parameter is string s && double.TryParse(s, out double parsed))
        {
            width = parsed;
        }
        return value is true ? new GridLength(width) : new GridLength(0);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) =>
        value is GridLength gl && gl.Value > 0;
}

