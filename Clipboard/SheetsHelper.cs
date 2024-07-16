namespace SunamoGoogleSheets.Clipboard;

public class SheetsHelper
{
    public static char? FirstLetterFromSheet(string item2)
    {
        if (item2.Length > 2)
        {

            if (item2[1] == AllChars.space)
            {
                return item2[0];
            }
        }
        return null;
    }

    public static string SwitchRowsAndColumn(string s, bool keepInSizeOfSmallest = true)
    {
        List<List<string>> exists = new List<List<string>>();

        var l = SHGetLines.GetLines(s);
        foreach (var item in l)
        {
            exists.Add(GetRowCells(item));
        }

        ValuesTableGrid<string> t = new ValuesTableGrid<string>(exists, keepInSizeOfSmallest);
        DataTable dt = t.SwitchRowsAndColumn();

        return DataTableToString(dt);
    }

    public static string DataTableToString(DataTable s)
    {
        StringBuilder sb = new StringBuilder();

        foreach (DataRow item in s.Rows)
        {
            sb.AppendLine(JoinForGoogleSheetRow(item.ItemArray));
        }

        return sb.ToString();
    }

    public static List<string> ColumnsIds(int count)
    {
        List<string> result = new List<string>();

        string prefixWith = "";

        while (count != 0)
        {
            for (char i = 'A'; i <= 'Z'; i++)
            {
                count--;
                result.Add(prefixWith + i);
                if (count == 0)
                {
                    break;
                }
            }

            if (prefixWith == "")
            {
                prefixWith = "A";
            }
            else
            {
                char ch = prefixWith[0];
                ch++;
                prefixWith = ch.ToString();
            }
        }

        return result;
    }

    /// <summary>
    /// A2 was = true
    /// </summary>
    /// <param name="input"></param>
    /// <param name="mustBeAllNumbers"></param>
    /// <param name="NHCalculateMedianAverage"></param>
    /// <returns></returns>
    public static string CalculateMedianAverage(string input, bool mustBeAllNumbers, Func<List<double>, string> NHCalculateMedianAverage)
    {
        var ls = Rows(input);

        StringBuilder sb = new StringBuilder();

        foreach (var item in ls)
        {
            var defDouble = -1;
            var list = CAToNumber.ToNumber<double>(BTS.ParseDouble, SheetsHelper.SplitFromGoogleSheets(item), defDouble, false);

            sb.AppendLine(NHCalculateMedianAverage(list));
        }

        return sb.ToString();
    }

    public static string CalculateMedianFromTwoRows(string s, Func<List<double>, string> NHCalculateMedianAverage)
    {
        var r = Rows(s);
        for (int i = 0; i < r.Count; i++)
        {
            r[i] = CalculateMedianAverage(r[i], true, NHCalculateMedianAverage);
        }
        return string.Join(Environment.NewLine, r);
    }

    public static List<List<string>> AllLines(string d)
    {
        List<List<string>> result = new List<List<string>>();
        var l = SHGetLines.GetLines(d);
        foreach (var item in l)
        {
            result.Add(GetRowCells(item));
        }
        return result;
    }

    public static List<string> GetRowCells(string ClipboardS)
    {
        return SplitFromGoogleSheets(ClipboardS);
    }

    /// <summary>
    /// If null, will be  load from clipboard
    /// </summary>
    /// <param name="input"></param>
    public static List<string> Rows(string input)
    {
        //if (input == null)
        //{
        //    input = ClipboardHelper.GetText();
        //}

        return input.Split(AllChars.nl).ToList(); //SHSplit.SplitMore(input, "\n");
    }

    /// <summary>
    /// A1 can be null
    /// </summary>
    /// <param name="input"></param>
    /// <param name=""></param>
    /// <returns></returns>
    public static List<string> SplitFromGoogleSheetsRow(string input, bool removeEmptyElements)
    {
        //if (input == null)
        //{
        //    input = ClipboardHelper.GetText();
        //}
        var r = SplitFromGoogleSheets(input);
        if (removeEmptyElements)
        {
            r = r.Where(d => !string.IsNullOrWhiteSpace(d)).ToList();
            //CA.RemoveStringsEmpty2(r);
        }
        return r;
    }

    public static List<string> SplitFromGoogleSheets2(string input)
    {
        return SHGetLines.GetLines(input);
    }

    /// <summary>
    /// rozděluje podle tab / space, pokud to chci podle \r\n tak SplitFromGoogleSheets2
    ///
    /// If A1 null, take from clipboard
    /// Not to parse whole content
    /// </summary>
    /// <param name="input"></param>
    public static List<string> SplitFromGoogleSheets(string input)
    {
        var bm = _sunamo.SunamoString.SH.TabOrSpaceNextTo(input);
        List<string> vr = new List<string>();

        if (bm.Count > 0)
        {
            vr.AddRange(SHSplit.SplitByIndexes(input, bm));
            vr.Reverse();
        }
        else
        {
            //ThisApp.Warning( "Bad data in clipboard");
            vr.Add(input);
        }
        var vr2 = SHSplit.SplitMore(input, AllStrings.tab);
        return vr2;
    }

    /// <summary>
    /// A1 are column names for ValuesTableGrid (not letter sorted a,b,.. but left column (Name, Rating, etc.)
    /// A2 are data
    /// </summary>
    /// <param name="captions"></param>
    /// <param name="exists"></param>
    public static string SwitchForGoogleSheets(List<string> captions, List<List<string>> exists)
    {
        ValuesTableGrid<string> vtg = new ValuesTableGrid<string>(exists);
        vtg.captions = captions;
        DataTable dt = vtg.SwitchRowsAndColumn();
        StringBuilder sb = new StringBuilder();
        foreach (DataRow item in dt.Rows)
        {
            JoinForGoogleSheetRow(sb, item.ItemArray);
        }
        string vr = sb.ToString();
        //////DebugLogger.Instance.WriteLine(vr);
        return vr;
    }

    public static void JoinForGoogleSheetRow(StringBuilder sb, IList en)
    {
        sb.AppendLine(JoinForGoogleSheetRow(en));
    }
    public static string JoinForGoogleSheetRow(IList en)
    {
        return string.Join(AllChars.tab, en);
    }

    //public static void JoinForGoogleSheetRow(StringBuilder sb, IList en)
    //{
    //    SheetsHelper.JoinForGoogleSheetRow(sb, en);
    //}

    //public static string JoinForGoogleSheetRow(IList en)
    //{
    //    return SheetsHelper.JoinForGoogleSheetRow(en);
    //}
}
