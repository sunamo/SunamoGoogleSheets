namespace SunamoGoogleSheets._sunamo.SunamoStringGetLines;

/// <summary>
/// Provides utilities for splitting text into lines
/// </summary>
internal class SHGetLines
{
    /// <summary>
    /// Splits text into lines, handling various newline formats (Windows, Unix, Mac)
    /// </summary>
    /// <param name="text">The text to split into lines</param>
    /// <returns>List of lines</returns>
    internal static List<string> GetLines(string text)
    {
        var parts = text.Split(new[] { "\r\n", "\n\r" }, StringSplitOptions.None).ToList();
        SplitByUnixNewline(parts);
        return parts;
    }

    /// <summary>
    /// Further splits lines by Unix-style newlines (\r and \n separately)
    /// </summary>
    /// <param name="lines">The list of lines to split</param>
    private static void SplitByUnixNewline(List<string> lines)
    {
        SplitBy(lines, "\r");
        SplitBy(lines, "\n");
    }

    /// <summary>
    /// Splits lines by the specified delimiter
    /// </summary>
    /// <param name="lines">The list of lines to split</param>
    /// <param name="delimiter">The delimiter to split by</param>
    private static void SplitBy(List<string> lines, string delimiter)
    {
        for (var i = lines.Count - 1; i >= 0; i--)
        {
            if (delimiter == "\r")
            {
                var linesByRN = lines[i].Split(new[] { "\r\n" }, StringSplitOptions.None);
                var linesByNR = lines[i].Split(new[] { "\n\r" }, StringSplitOptions.None);

                if (linesByRN.Length > 1)
                    ThrowEx.Custom("cannot contain any \r\name, pass already split by this pattern");
                else if (linesByNR.Length > 1) ThrowEx.Custom("cannot contain any \n\r, pass already split by this pattern");
            }

            var parts = lines[i].Split(new[] { delimiter }, StringSplitOptions.None);

            if (parts.Length > 1) InsertOnIndex(lines, parts.ToList(), i);
        }
    }

    /// <summary>
    /// Inserts items into the list at the specified index, replacing the item at that index
    /// </summary>
    /// <param name="lines">The list to insert into</param>
    /// <param name="itemsToInsert">The items to insert</param>
    /// <param name="index">The index to insert at</param>
    private static void InsertOnIndex(List<string> lines, List<string> itemsToInsert, int index)
    {
        itemsToInsert.Reverse();

        lines.RemoveAt(index);

        foreach (var item in itemsToInsert) lines.Insert(index, item);
    }
}