namespace SunamoGoogleSheets._sunamo.SunamoBts;

/// <summary>
/// Provides basic type conversion and parsing utilities
/// </summary>
internal class BTS
{
    /// <summary>
    /// Parses a double value from text, removing spaces before parsing
    /// </summary>
    /// <param name="text">The text to parse</param>
    /// <param name="defaultValue">The default value to return if parsing fails</param>
    /// <returns>The parsed double value, or the default value if parsing fails</returns>
    internal static double ParseDouble(string text, double defaultValue)
    {
        text = text.Replace(" ", string.Empty);

        double parsedValue = 0;
        if (double.TryParse(text, out parsedValue)) return parsedValue;
        return defaultValue;
    }
}