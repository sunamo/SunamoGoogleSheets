namespace SunamoGoogleSheets._sunamo.SunamoBts;

internal class BTS
{
    internal static double ParseDouble(string text, double defaultValue)
    {
        text = text.Replace(" ", string.Empty);

        if (double.TryParse(text, out var parsedValue)) return parsedValue;
        return defaultValue;
    }
}
