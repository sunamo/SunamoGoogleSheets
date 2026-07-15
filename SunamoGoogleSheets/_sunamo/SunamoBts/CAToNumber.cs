namespace SunamoGoogleSheets._sunamo.SunamoBts;

internal class CAToNumber
{
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
                    ThrowEx.BadFormatOfElementInList(item ?? "<null>", nameof(list), SH.NullToStringOrDefault);
                    throw new InvalidOperationException("Failed to parse all numbers");
                }

            if (!EqualityComparer<T>.Default.Equals(number, defaultValue)) result.Add(number);
        }

        return result;
    }
}
