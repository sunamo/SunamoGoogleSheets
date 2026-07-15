namespace SunamoGoogleSheets._public.SunamoData.Data;

public class FromToTGoogleSheets<T> : FromToTSHGoogleSheets<T> where T : struct
{
    public FromToTGoogleSheets()
    {
        var type = typeof(T);
        if (type == typeof(int)) TimestampFormat = FromToUseGoogleSheets.None;
    }

    private FromToTGoogleSheets(bool isEmpty) : this()
    {
        this.Empty = isEmpty;
    }

    public FromToTGoogleSheets(T fromValue, T toValue, FromToUseGoogleSheets timestampFormat = FromToUseGoogleSheets.DateTime) : this()
    {
        this.From = fromValue;
        this.To = toValue;
        this.TimestampFormat = timestampFormat;
    }

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

    public bool IsFilledWithData()
    {
        return ToL >= 0 && ToL != 0;
    }

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
            if (int.TryParse(timeText, out var parsedHours)) result += parsedHours * (int)DTConstants.SecondsInHour;
        }

        return result;
    }

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

    protected virtual string ToStringDateTime(LangsGoogleSheets lang)
    {
        return "";
    }
}
