namespace SunamoGoogleSheets;


/// <summary>
/// Contains methods which was earlier in FromToT
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
public class FromToTGoogleSheets<T> : FromToTSHGoogleSheets<T> where T : struct
{
    internal FromToTGoogleSheets()
    {
        var t = typeof(T);
        if (t == Types.tInt)
        {
            ftUse = FromToUseGoogleSheets.None;
        }
    }
    /// <summary>
    /// Use Empty contstant outside of class
    /// </summary>
    /// <param name="empty"></param>
    private FromToTGoogleSheets(bool empty) : this()
    {
        this.empty = empty;
    }
    /// <summary>
    /// A3 true = DateTime
    /// A3 False = None
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="ftUse"></param>
    internal FromToTGoogleSheets(T from, T to, FromToUseGoogleSheets ftUse = FromToUseGoogleSheets.DateTime) : this()
    {
        this.from = from;
        this.to = to;
        this.ftUse = ftUse;
    }
    /// <summary>
    /// After it could be called IsFilledWithData
    /// </summary>
    /// <param name="input"></param>
    internal void Parse(string input)
    {
        List<string> v = null;
        if (input.Contains(AllStrings.dash))
        {
            v = input.Split(AllChars.dash).ToList(); //SHSplit.SplitChar(input, new Char[] { AllChars.dash });
        }
        else
        {
            v = new List<string>(new String[] { input });
        }
        if (v[0] == "0")
        {
            v[0] = "00:01";
        }
        if (v[1] == "24")
        {
            v[1] = "23:59";
        }
        var v0 = (long)ReturnSecondsFromTimeFormat(v[0]);
        fromL = v0;
        if (v.Count > 1)
        {
            var v1 = (long)ReturnSecondsFromTimeFormat(v[1]);
            toL = v1;
        }
    }
    internal bool IsFilledWithData()
    {
        //from != 0 && - cant be, if entered 0-24 fails
        return toL >= 0 && toL != 0;
    }
    /// <summary>
    /// Use DTHelperCs.ToShortTimeFromSeconds to convert back
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    private int ReturnSecondsFromTimeFormat(string v)
    {
        int result = 0;
        if (v.Contains(AllStrings.colon))
        {
            var parts = v.Split(AllChars.colon).ToList().ConvertAll(d => int.Parse(d)); //SHSplit.SplitToIntList(v, new String[] { AllStrings.colon });
            result += parts[0] * (int)DTConstants.secondsInHour;
            if (parts.Count > 1)
            {
                result += parts[1] * (int)DTConstants.secondsInMinute;
            }
        }
        else
        {
            if (int.TryParse(v, out var _))
            {
                result += int.Parse(v) * (int)DTConstants.secondsInHour;
            }
        }
        return result;
    }
    internal string ToString(LangsGoogleSheets l)
    {
        if (empty)
        {
            return string.Empty;
        }
        else
        {
            if (new List<FromToUseGoogleSheets>([FromToUseGoogleSheets.DateTime, FromToUseGoogleSheets.Unix, FromToUseGoogleSheets.UnixJustTime]).Any(d => d == ftUse))
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
    }
    protected virtual string ToStringDateTime(LangsGoogleSheets l)
    {
        return "";
    }
}