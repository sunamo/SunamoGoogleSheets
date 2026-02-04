namespace SunamoGoogleSheets._public.SunamoEnums.Enums;

/// <summary>
/// Specifies the format to use when converting from/to timestamps in Google Sheets
/// </summary>
public enum FromToUseGoogleSheets
{
    /// <summary>Use DateTime format</summary>
    DateTime,
    /// <summary>Use Unix timestamp format</summary>
    Unix,
    /// <summary>Use Unix timestamp format for time only</summary>
    UnixJustTime,
    /// <summary>No timestamp conversion</summary>
    None
}