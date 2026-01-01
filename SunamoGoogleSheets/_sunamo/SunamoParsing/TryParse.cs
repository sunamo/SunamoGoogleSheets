namespace SunamoGoogleSheets._sunamo.SunamoParsing;

/// <summary>
/// Provides try-parse utilities for various types
/// </summary>
internal class TryParse
{
    /// <summary>
    /// DateTime parsing utilities (moved to SunamoDateTime project)
    /// </summary>
    internal class DateTime
    {
    }

    /// <summary>
    /// Integer parsing utilities
    /// </summary>
    internal class Integer
    {
        /// <summary>
        /// Gets the singleton instance
        /// </summary>
        internal static Integer Instance = new();

        /// <summary>
        /// Gets or sets the last successfully parsed integer value
        /// </summary>
        internal int LastInt { get; set; } = -1;
    }
}