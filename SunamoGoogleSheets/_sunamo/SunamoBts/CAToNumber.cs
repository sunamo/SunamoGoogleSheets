namespace SunamoGoogleSheets._sunamo.SunamoBts;

/// <summary>
/// Provides collection-to-number conversion utilities
/// </summary>
internal class CAToNumber
{
    /// <summary>
    /// Converts a list of items to a list of numbers using the specified parse function
    /// </summary>
    /// <typeparam name="T">The numeric type to convert to</typeparam>
    /// <param name="parseFunction">The function to parse each item to the numeric type</param>
    /// <param name="list">The list of items to convert</param>
    /// <param name="defaultValue">The default value used when parsing fails</param>
    /// <param name="isRequiringAllNumbers">If true, throws an exception when any item fails to parse; if false, skips failed items</param>
    /// <returns>A list of successfully parsed numbers</returns>
    internal static List<T> ToNumber<T>(Func<string, T, T> parseFunction, IList list, T defaultValue,
        bool isRequiringAllNumbers = true)
    {
        var result = new List<T>();
        foreach (var item in list)
        {
            var number = parseFunction.Invoke(item?.ToString() ?? string.Empty, defaultValue);
            if (isRequiringAllNumbers)
                if (EqualityComparer<T>.Default.Equals(number, defaultValue))
                {
                    ThrowEx.BadFormatOfElementInList(item, nameof(list), SH.NullToStringOrDefault);
                    throw new InvalidOperationException("Failed to parse all numbers");
                }

            if (!EqualityComparer<T>.Default.Equals(number, defaultValue)) result.Add(number);
        }

        return result;
    }
}