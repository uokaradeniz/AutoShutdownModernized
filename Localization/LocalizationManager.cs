using System.Globalization;
using System.Windows;

namespace AutoShutdownModernized.Localization;

public static class LocalizationManager
{
    public static CultureInfo ApplySupportedCulture(CultureInfo culture)
    {
        var selectedCulture = culture.TwoLetterISOLanguageName.Equals("tr", StringComparison.OrdinalIgnoreCase)
            ? new CultureInfo("tr-TR")
            : new CultureInfo("en-US");

        CultureInfo.DefaultThreadCurrentCulture = selectedCulture;
        CultureInfo.DefaultThreadCurrentUICulture = selectedCulture;
        CultureInfo.CurrentCulture = selectedCulture;
        CultureInfo.CurrentUICulture = selectedCulture;

        var app = System.Windows.Application.Current;
        if (app is not null)
        {
            app.Resources.MergedDictionaries.Clear();
            app.Resources.MergedDictionaries.Add(new ResourceDictionary
            {
                Source = new Uri($"pack://application:,,,/Localization/Strings.{selectedCulture.Name}.xaml", UriKind.Absolute)
            });
        }

        return selectedCulture;
    }
}


