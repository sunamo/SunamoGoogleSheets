namespace SunamoGoogleSheets._sunamo.SunamoInterfaces.Interfaces;

/// <summary>
/// Defines a contract for parsing text content
/// </summary>
internal interface IParser
{
    /// <summary>
    /// Parses the specified text content
    /// </summary>
    /// <param name="text">The text to parse</param>
    void Parse(string text);
}