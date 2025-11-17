// EN: Variable names have been checked and replaced with self-descriptive names
// CZ: Názvy proměnných byly zkontrolovány a nahrazeny samopopisnými názvy

namespace SunamoGoogleSheets._public.SunamoData.Data;

public class FromToTGoogleSheets<T> : FromToTSHGoogleSheets<T> where T : struct
{
    public FromToTGoogleSheets()
    {
        var type = typeof(T);
        if (type == typeof(int)) ftUse = FromToUseGoogleSheets.None;
    }


    private FromToTGoogleSheets(bool empty) : this()
    {
        this.empty = empty;
    }


    public FromToTGoogleSheets(T from, T to, FromToUseGoogleSheets ftUse = FromToUseGoogleSheets.DateTime) : this()
    {
        this.from = from;
        this.to = to;
        this.ftUse = ftUse;
    }


    public void Parse(string input)
    {
        List<string> v = null;
        if (input.Contains("-"))
            v = input.Split('-').ToList();
        else
            v = new List<string>(new[] { input });
        if (v[0] == "0") v[0] = "00:01";
        if (v[1] == "24") v[1] = "23:59";
        var v0 = (long)ReturnSecondsFromTimeFormat(v[0]);
        fromL = v0;
        if (v.Count > 1)
        {
            var v1 = (long)ReturnSecondsFromTimeFormat(v[1]);
            toL = v1;
        }
    }

    public bool IsFilledWithData()
    {
        return toL >= 0 && toL != 0;
    }


    private int ReturnSecondsFromTimeFormat(string v)
    {
        var result = 0;
        if (v.Contains(":"))
        {
            var parts = v.Split(':').ToList().ConvertAll(d => int.Parse(d));
            result += parts[0] * (int)DTConstants.secondsInHour;
            if (parts.Count > 1) result += parts[1] * (int)DTConstants.secondsInMinute;
        }
        else
        {
            if (int.TryParse(v, out var _)) result += int.Parse(v) * (int)DTConstants.secondsInHour;
        }

        return result;
    }

    public string ToString(LangsGoogleSheets l)
    {
        if (empty) return string.Empty;

        if (new List<FromToUseGoogleSheets>([FromToUseGoogleSheets.DateTime, FromToUseGoogleSheets.Unix,
                FromToUseGoogleSheets.UnixJustTime]).Any(d => d == ftUse))
        {
            return ToStringDateTime(l);
        }
        else if (ftUse == FromToUseGoogleSheets.None)
        {
            return from + "-" + to;
        }
        else
        {
            ThrowEx.NotImplementedCase(ftUse);
            return string.Empty;
        }
    }

    protected virtual string ToStringDateTime(LangsGoogleSheets l)
    {
        return "";
    }
}