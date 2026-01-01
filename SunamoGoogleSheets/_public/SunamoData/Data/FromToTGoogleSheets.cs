namespace SunamoGoogleSheets._public.SunamoData.Data;

/// <summary>
/// Represents a range (from-to) of values with Google Sheets formatting support
/// </summary>
/// <typeparam name="T">The type of values in the range (must be a struct)</typeparam>
public class FromToTGoogleSheets<T> : FromToTSHGoogleSheets<T> where T : struct
{
    /// <summary>
    /// Initializes a new instance of the FromToTGoogleSheets class
    /// </summary>
    public FromToTGoogleSheets()
    {
        var type = typeof(T);
        if (type == typeof(int)) FtUse = FromToUseGoogleSheets.None;
    }

    /// <summary>
    /// Initializes a new empty instance
    /// </summary>
    /// <param name="isEmpty">Whether this instance is empty</param>
    private FromToTGoogleSheets(bool isEmpty) : this()
    {
        this.Empty = isEmpty;
    }

    /// <summary>
    /// Initializes a new instance with from and to values
    /// </summary>
    /// <param name="fromValue">The from value</param>
    /// <param name="toValue">The to value</param>
    /// <param name="ftUse">The format to use when converting timestamps</param>
    public FromToTGoogleSheets(T fromValue, T toValue, FromToUseGoogleSheets ftUse = FromToUseGoogleSheets.DateTime) : this()
    {
        this.From = fromValue;
        this.To = toValue;
        this.FtUse = ftUse;
    }

    /// <summary>
    /// Parses a time range from text (e.g., "0-24" or "10:30-15:45")
    /// </summary>
    /// <param name="input">The text to parse</param>
    public void Parse(string input)
    {
        List<string> parts = null;
        if (input.Contains("-"))
            parts = input.Split('-').ToList();
        else
            parts = new List<string>(new[] { input });
        if (parts[0] == "0") parts[0] = "00:01";
        if (parts[1] == "24") parts[1] = "23:59";
        var fromSeconds = (long)ReturnSecondsFromTimeFormat(parts[0]);
        var toSeconds = fromSeconds;
        if (parts.Count > 1)
        {
            toSeconds = (long)ReturnSecondsFromTimeFormat(parts[1]);
        }

        From = (T)(dynamic)fromSeconds;
        To = (T)(dynamic)toSeconds;
    }

    /// <summary>
    /// Checks if this range is filled with data
    /// </summary>
    /// <returns>True if the range has data, false otherwise</returns>
    public bool IsFilledWithData()
    {
        return ToL >= 0 && ToL != 0;
    }

    /// <summary>
    /// Converts a time format string (HH:MM) to seconds
    /// </summary>
    /// <param name="timeText">The time text to convert</param>
    /// <returns>The number of seconds</returns>
    private int ReturnSecondsFromTimeFormat(string timeText)
    {
        var result = 0;
        if (timeText.Contains(":"))
        {
            var parts = timeText.Split(':').ToList().ConvertAll(part => int.Parse(part));
            result += parts[0] * (int)DTConstants.SecondsInHour;
            if (parts.Count > 1) result += parts[1] * (int)DTConstants.SecondsInMinute;
        }
        else
        {
            if (int.TryParse(timeText, out var _)) result += int.Parse(timeText) * (int)DTConstants.SecondsInHour;
        }

        return result;
    }

    /// <summary>
    /// Converts this range to a string representation
    /// </summary>
    /// <param name="lang">The language to use for formatting</param>
    /// <returns>String representation of this range</returns>
    public string ToString(LangsGoogleSheets lang)
    {
        if (Empty) return string.Empty;

        if (new List<FromToUseGoogleSheets>([FromToUseGoogleSheets.DateTime, FromToUseGoogleSheets.Unix,
                FromToUseGoogleSheets.UnixJustTime]).Any(useType => useType == FtUse))
        {
            return ToStringDateTime(lang);
        }
        else if (FtUse == FromToUseGoogleSheets.None)
        {
            return From + "-" + To;
        }
        else
        {
            ThrowEx.NotImplementedCase(FtUse);
            return string.Empty;
        }
    }

    /// <summary>
    /// Converts this range to a DateTime string representation
    /// </summary>
    /// <param name="lang">The language to use for formatting</param>
    /// <returns>DateTime string representation</returns>
    protected virtual string ToStringDateTime(LangsGoogleSheets lang)
    {
        return "";
    }
}