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
        if (type == typeof(int)) TimestampFormat = FromToUseGoogleSheets.None;
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
    /// <param name="timestampFormat">The format to use when converting timestamps</param>
    public FromToTGoogleSheets(T fromValue, T toValue, FromToUseGoogleSheets timestampFormat = FromToUseGoogleSheets.DateTime) : this()
    {
        this.From = fromValue;
        this.To = toValue;
        this.TimestampFormat = timestampFormat;
    }

    /// <summary>
    /// Parses a time range from text (e.g., "0-24" or "10:30-15:45")
    /// </summary>
    /// <param name="input">The text to parse</param>
    public void Parse(string input)
    {
        List<string> timeParts;
        if (input.Contains("-"))
            timeParts = input.Split('-').ToList();
        else
            timeParts = new List<string>(new[] { input });
        if (timeParts[0] == "0") timeParts[0] = "00:01";
        if (timeParts[1] == "24") timeParts[1] = "23:59";
        var fromSeconds = (long)ReturnSecondsFromTimeFormat(timeParts[0]);
        var toSeconds = fromSeconds;
        if (timeParts.Count > 1)
        {
            toSeconds = (long)ReturnSecondsFromTimeFormat(timeParts[1]);
        }

        From = (T)(dynamic)fromSeconds!;
        To = (T)(dynamic)toSeconds!;
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
            var timeComponents = timeText.Split(':').ToList().ConvertAll(part => int.Parse(part));
            result += timeComponents[0] * (int)DTConstants.SecondsInHour;
            if (timeComponents.Count > 1) result += timeComponents[1] * (int)DTConstants.SecondsInMinute;
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
                FromToUseGoogleSheets.UnixJustTime]).Any(useType => useType == TimestampFormat))
        {
            return ToStringDateTime(lang);
        }
        else if (TimestampFormat == FromToUseGoogleSheets.None)
        {
            return From + "-" + To;
        }
        else
        {
            ThrowEx.NotImplementedCase(TimestampFormat);
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