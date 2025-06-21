using System.Globalization;
using ZXing.Net.Maui;

namespace StoreIt.Maui.Converters;

public class IsStringNotNullOrEmptyConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return !string.IsNullOrEmpty(value?.ToString());
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class BoolToStarConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if(value is bool b)
            return b ? "★" : "☆";

        return string.Empty;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class BoolToColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if(value is bool b)
            return b ? Colors.Gold : Colors.Gray;

        return Colors.Gray;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class StringToBarcodeFormatConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (Enum.TryParse<BarcodeFormat>(value?.ToString(), out var format))
        {
            return format;
        }
        return BarcodeFormat.QrCode; // Default fallback
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value?.ToString();
    }
}

public class BoolToCardTypeBackgroundConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        bool isSelected = (bool)(value ?? false);
        if (Application.Current?.RequestedTheme == AppTheme.Dark)
        {
            //ToDo: take color from resources
            return isSelected ? Color.FromArgb("#1565C0") : Color.FromArgb("#1E1E1E"); //OffBlack
        }
        else
        {
            return isSelected ? Color.FromArgb("#E3F2FD") : Color.FromArgb("#FFFFFF");
        }
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class BoolToCardTypeBorderConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        //ToDo: take color from resources
        bool isSelected = (bool)(value ?? false);
        
        if (Application.Current?.RequestedTheme == AppTheme.Dark)
        {
            return isSelected ? Color.FromArgb("#2196F3") : Color.FromArgb("#404040");
        }
        else
        {
            return isSelected ? Color.FromArgb("#2196F3") : Color.FromArgb("#E0E0E0");
        }
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class BoolToCardTypeTextColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        //ToDo: take color from resources
        bool isSelected = (bool)(value ?? false);
        if (Application.Current?.RequestedTheme == AppTheme.Dark)
        {
            return isSelected ? Colors.White : Color.FromArgb("#AAA");
        }
        else
        {
            return isSelected ? Colors.Black : Color.FromArgb("#666");
        }
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class BoolToThemeBackgroundConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        //ToDo: take color from resources
        bool isSelected = (bool)(value ?? false);
        if (Application.Current?.RequestedTheme == AppTheme.Dark)
        {
            return isSelected ? Color.FromArgb("#1565C0") : Color.FromArgb("#2A2A2A");
        }
        else
        {
            return isSelected ? Color.FromArgb("#E3F2FD") : Color.FromArgb("#FFFFFF");
        }
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class BoolToThemeBorderConverter : IValueConverter
{
    //ToDo: take color from resources
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        bool isSelected = (bool)(value ?? false);

        if (Application.Current?.RequestedTheme == AppTheme.Dark)
        {
            return isSelected ? Color.FromArgb("#2196F3") : Color.FromArgb("#444");
        }
        else
        {
            return isSelected ? Color.FromArgb("#2196F3") : Color.FromArgb("#DEE2E6");
        }
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class BoolToThemeTextColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        //ToDo: take color from resources
        bool isSelected = (bool)(value ?? false);
        if (Application.Current?.RequestedTheme == AppTheme.Dark)
        {
            return isSelected ? Colors.White : Color.FromArgb("#AAA");
        }
        else
        {
            return isSelected ? Colors.Black : Color.FromArgb("#333");
        }
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
