using SunamoThisApp;

namespace SunamoGoogleSheets.Data;



/// <summary>
/// Měl by být ve SunamoDateTime protože často užívá DTHelper*
///
/// Contains methods which was earlier in FromToT
/// TryParse je ve SunamoParsing
/// TryParse se používá ve SunamoDateTime
///
/// čili musím prvně zkompilovat SunamoParsing abych mohl užívat TryParse. v SunamoDateTime
///
/// jelikož se FromToT používá jen ve SheetsTable, dokončím prvně výše 2 jmenované a ty potom přilinkuji případně do SunamoGoogleSheets
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
        if (input.Contains(AllStringsSE.dash))
        {
            v = SHSplit.SplitChar(input, new Char[] { AllCharsSE.dash });
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
        if (v.Contains(AllStringsSE.colon))
        {
            var parts = SHSplit.SplitToIntList(v, new String[] { AllStringsSE.colon });
            result += parts[0] * (int)DTConstants.secondsInHour;
            if (parts.Count > 1)
            {
                result += parts[1] * (int)DTConstants.secondsInMinute;
            }
        }
        else
        {
            if (BTSSE.IsInt(v))
            {
                result += int.Parse(v) * (int)DTConstants.secondsInHour;
            }
        }
        return result;
    }

    public override string ToString()
    {
        if (empty)
        {
            return string.Empty;
        }
        else
        {
            if (ftUse == FromToUse.DateTime)
            {
                var from2 = DTHelperCs.ToShortTimeFromSeconds(fromL);
                if (toL != 0)
                {
                    return $"{from2}-{DTHelperCs.ToShortTimeFromSeconds(toL)}";
                }
                return $"{from2}";
            }
            else if (ftUse == FromToUse.Unix)
            {

                var from2 = UnixDateConverter.From(fromL);
                var from3 = DTHelperMulti.DateTimeToString(from2, ThisApp.l, DTConstants.UnixFsStart);
                if (toL != 0)
                {
                    return $"{from3}-{DTHelperMulti.DateTimeToString(UnixDateConverter.From(toL), ThisApp.l, DTConstants.UnixFsStart)}";
                }
                return $"{from3}";
            }
            else if (ftUse == FromToUse.UnixJustTime)
            {
                var from2 = UnixDateConverter.From(fromL);
                var from3 = DTHelperMulti.TimeToString(from2, ThisApp.l, DTConstants.UnixFsStart);
                if (toL != 0)
                {
                    return $"{from3}-{DTHelperMulti.TimeToString(UnixDateConverter.From(toL), ThisApp.l, DTConstants.UnixFsStart)}";
                }
                return $"{from3}";
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
}
