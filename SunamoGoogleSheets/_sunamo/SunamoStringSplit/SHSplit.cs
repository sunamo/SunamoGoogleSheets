namespace SunamoGoogleSheets._sunamo.SunamoStringSplit;

/// <summary>
/// Provides string splitting utilities
/// </summary>
internal class SHSplit
{
    /// <summary>
    /// Splits text by delimiters, removing empty entries
    /// </summary>
    /// <param name="text">The text to split</param>
    /// <param name="delimiters">The delimiters to split by</param>
    /// <returns>List of non-empty parts</returns>
    internal static List<string> Split(string text, params string[] delimiters)
    {
        return text.Split(delimiters, StringSplitOptions.RemoveEmptyEntries).ToList();
    }

    /// <summary>
    /// Splits text by delimiters, keeping empty entries
    /// </summary>
    /// <param name="text">The text to split</param>
    /// <param name="delimiters">The delimiters to split by</param>
    /// <returns>List of all parts including empty ones</returns>
    internal static List<string> SplitNone(string text, params string[] delimiters)
    {
        return text.Split(delimiters, StringSplitOptions.None).ToList();
    }
}