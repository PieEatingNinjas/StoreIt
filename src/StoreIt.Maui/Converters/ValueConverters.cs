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
        if (value is bool b)
            return b ? "★" : "☆";

        return string.Empty;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class InverseBoolConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool b)
            return !b;

        return false;
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
        if (value is bool b && Application.Current?.Resources is not null)
        {
            var resources = Application.Current.Resources;
            if (b)
                return resources["FavoriteColor"];
            else
                return resources["UnfavoriteColor"];
        }

        // Fallback
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

        if (Application.Current?.Resources is not null)
        {
            var resources = Application.Current.Resources;
            if (Application.Current?.RequestedTheme == AppTheme.Dark)
            {
                return isSelected ? resources["SelectionBackgroundDark"] : resources["UnselectedBackgroundDark"];
            }
            else
            {
                return isSelected ? resources["SelectionBackgroundLight"] : resources["UnselectedBackgroundLight"];
            }
        }

        // Fallback
        return Colors.Gray;
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
        bool isSelected = (bool)(value ?? false);

        if (Application.Current?.Resources is not null)
        {
            var resources = Application.Current.Resources;
            if (Application.Current?.RequestedTheme == AppTheme.Dark)
            {
                return isSelected ? resources["SelectionBorderDark"] : resources["UnselectedBorderDark"];
            }
            else
            {
                return isSelected ? resources["SelectionBorderLight"] : resources["UnselectedBorderLight"];
            }
        }

        // Fallback
        return Colors.Gray;
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
        bool isSelected = (bool)(value ?? false);

        if (Application.Current?.Resources is not null)
        {
            var resources = Application.Current.Resources;
            if (Application.Current?.RequestedTheme == AppTheme.Dark)
            {
                return isSelected ? resources["SelectionTextDark"] : resources["UnselectedTextDark"];
            }
            else
            {
                return isSelected ? resources["SelectionTextLight"] : resources["UnselectedTextLight"];
            }
        }

        // Fallback
        return Colors.Gray;
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
        bool isSelected = (bool)(value ?? false);

        if (Application.Current?.Resources is not null)
        {
            var resources = Application.Current.Resources;
            if (Application.Current?.RequestedTheme == AppTheme.Dark)
            {
                return isSelected ? resources["SelectionBackgroundDark"] : resources["UnselectedBackgroundDark"];
            }
            else
            {
                return isSelected ? resources["SelectionBackgroundLight"] : resources["UnselectedBackgroundLight"];
            }
        }

        // Fallback
        return Colors.Gray;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class BoolToThemeBorderConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        bool isSelected = (bool)(value ?? false);

        if (Application.Current?.Resources is not null)
        {
            var resources = Application.Current.Resources;
            if (Application.Current?.RequestedTheme == AppTheme.Dark)
            {
                return isSelected ? resources["SelectionBorderDark"] : resources["UnselectedBorderDark"];
            }
            else
            {
                return isSelected ? resources["SelectionBorderLight"] : resources["UnselectedBorderLight"];
            }
        }

        // Fallback
        return Colors.Gray;
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
        bool isSelected = (bool)(value ?? false);

        if (Application.Current?.Resources is not null)
        {
            var resources = Application.Current.Resources;
            if (Application.Current?.RequestedTheme == AppTheme.Dark)
            {
                return isSelected ? resources["SelectionTextDark"] : resources["UnselectedTextDark"];
            }
            else
            {
                return isSelected ? resources["SelectionTextLight"] : resources["UnselectedTextLight"];
            }
        }

        // Fallback
        return Colors.Gray;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}