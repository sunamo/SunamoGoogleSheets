namespace SunamoGoogleSheets;


/// <summary>
/// Contains methods which was earlier in FromToT
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
public class FromToT<T> : FromToTSH<T>, IParser where T : struct
{
    public FromToT()
    {
        var t = typeof(T);
        if (t == Types.tInt)
        {
            ftUse = FromToUse.None;
        }
    }
    /// <summary>
    /// Use Empty contstant outside of class
    /// </summary>
    /// <param name="empty"></param>
    private FromToT(bool empty) : this()
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
    public FromToT(T from, T to, FromToUse ftUse = FromToUse.DateTime) : this()
    {
        this.from = from;
        this.to = to;
        this.ftUse = ftUse;
    }
    /// <summary>
    /// After it could be called IsFilledWithData
    /// </summary>
    /// <param name="input"></param>
    public void Parse(string input)
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
    public bool IsFilledWithData()
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
    public string ToString(Langs l)
    {
        if (empty)
        {
            return string.Empty;
        }
        else
        {
            if (new List<FromToUse>([FromToUse.DateTime, FromToUse.Unix, FromToUse.UnixJustTime]).Any(d => d == ftUse))
            {
                return ToStringDateTime(l);
            }
            else if (ftUse == FromToUse.None)
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
    protected virtual string ToStringDateTime(Langs l)
    {
        return "";
    }
}