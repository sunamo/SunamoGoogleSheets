namespace SunamoGoogleSheets._sunamo.SunamoString;

internal class SH
{
    internal static string NullToStringOrDefault(object value)
    {
        return value == null ? " " + "(null)" : " " + value;
    }

    internal static List<int> TabOrSpaceNextTo(string text)
    {
        var indices = new List<int>();

        indices.AddRange(ReturnOccurencesOfString(text, "\t"));
        indices.AddRange(ReturnOccurencesOfString(text, " "));

        indices.Sort();

        return indices;
    }

    internal static List<int> ReturnOccurencesOfString(string text, string searchPattern)
    {
        var results = new List<int>();
        for (var index = 0; index < text.Length - searchPattern.Length + 1; index++)
        {
            var substring = text.Substring(index, searchPattern.Length);

            if (substring == searchPattern)
                results.Add(index);
        }

        return results;
    }
}
