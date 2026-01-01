namespace SunamoGoogleSheets._sunamo.SunamoString;

/// <summary>
/// Provides string manipulation utilities
/// </summary>
internal class SH
{
    /// <summary>
    /// Converts a value to string, returning "(null)" if the value is null
    /// </summary>
    /// <param name="value">The value to convert</param>
    /// <returns>String representation with leading space, or " (null)" if value is null</returns>
    internal static string NullToStringOrDefault(object value)
    {
        return value == null ? " " + "(null)" : " " + value;
    }

    /// <summary>
    /// Finds all positions of tab or space characters in the text
    /// </summary>
    /// <param name="text">The text to search in</param>
    /// <returns>Sorted list of indices where tabs or spaces occur</returns>
    internal static List<int> TabOrSpaceNextTo(string text)
    {
        var indices = new List<int>();

        indices.AddRange(ReturnOccurencesOfString(text, "\t"));
        indices.AddRange(ReturnOccurencesOfString(text, " "));

        indices.Sort();

        return indices;
    }

    /// <summary>
    /// Returns all positions where the search pattern occurs in the text
    /// </summary>
    /// <param name="text">The text to search in</param>
    /// <param name="searchPattern">The pattern to search for</param>
    /// <returns>List of indices where the pattern was found</returns>
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