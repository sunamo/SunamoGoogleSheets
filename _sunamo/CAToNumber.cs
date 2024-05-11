

namespace SunamoGoogleSheets;
internal class CAToNumber
{
    internal static List<T> ToNumber<T>(Func<string, T, T> parse, IList enumerable, T defVal, bool mustBeAllNumbers = true)
    {
        List<T> result = new List<T>();
        foreach (var item in enumerable)
        {
            var number = parse.Invoke(item.ToString(), defVal);
            if (mustBeAllNumbers)
            {
                if (EqualityComparer<T>.Default.Equals(number, defVal))
                {
                    ThrowEx.BadFormatOfElementInList(item, nameof(enumerable));
                    return null;
                }
            }

            if (!EqualityComparer<T>.Default.Equals(number, defVal))
            {
                result.Add(number);
            }
        }
        return result;
    }
}
