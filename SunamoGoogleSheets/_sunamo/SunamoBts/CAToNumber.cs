// EN: Variable names have been checked and replaced with self-descriptive names
// CZ: Názvy proměnných byly zkontrolovány a nahrazeny samopopisnými názvy
namespace SunamoGoogleSheets._sunamo.SunamoBts;

internal class CAToNumber
{
    internal static List<T> ToNumber<T>(Func<string, T, T> parse, IList enumerable, T defVal,
        bool mustBeAllNumbers = true)
    {
        var result = new List<T>();
        foreach (var item in enumerable)
        {
            var number = parse.Invoke(item.ToString(), defVal);
            if (mustBeAllNumbers)
                if (EqualityComparer<T>.Default.Equals(number, defVal))
                {
                    ThrowEx.BadFormatOfElementInList(item, nameof(enumerable), SH.NullToStringOrDefault);
                    return null;
                }

            if (!EqualityComparer<T>.Default.Equals(number, defVal)) result.Add(number);
        }

        return result;
    }
}