namespace SunamoGoogleSheets.Clipboard;

public class SheetsHelper
{
    public static char? FirstLetterFromSheet(string item2)
    {
        if (item2.Length > 2)
            if (item2[1] == ' ')
                return item2[0];
        return null;
    }
    public static string SwitchRowsAndColumn(string s, bool keepInSizeOfSmallest = true)
    {
        var exists = new List<List<string>>();
        var l = SHGetLines.GetLines(s);
        foreach (var item in l.Skip(1))
        {
            if (item.Trim() == "") continue;
            exists.Add(GetRowCells(item));
        }
        var t = new ValuesTableGrid<string>(exists, keepInSizeOfSmallest);
        t.captions = GetRowCells(l[0]);
        var dt = t.SwitchRowsAndColumn();
        return DataTableToString(dt);
    }
    public static string DataTableToString(DataTable s)
    {
        var sb = new StringBuilder();
        foreach (DataRow item in s.Rows) sb.AppendLine(JoinForGoogleSheetRow(item.ItemArray));
        return sb.ToString();
    }
    public static List<string> ColumnsIds(int count)
    {
        var result = new List<string>();
        var prefixWith = "";
        while (count != 0)
        {
            for (var i = 'A'; i <= 'Z'; i++)
            {
                count--;
                result.Add(prefixWith + i);
                if (count == 0) break;
            }
            if (prefixWith == "")
            {
                prefixWith = "A";
            }
            else
            {
                var ch = prefixWith[0];
                ch++;
                prefixWith = ch.ToString();
            }
        }
        return result;
    }
    /// <summary>
    ///     A2 was = true
    /// </summary>
    /// <param name="input"></param>
    /// <param name="mustBeAllNumbers"></param>
    /// <param name="NHCalculateMedianAverage"></param>
    /// <returns></returns>
    public static string CalculateMedianAverage(string input, bool mustBeAllNumbers,
        Func<List<double>, string> NHCalculateMedianAverage)
    {
        var ls = Rows(input);
        var sb = new StringBuilder();
        foreach (var item in ls)
        {
            var defDouble = -1;
            var list = CAToNumber.ToNumber<double>(BTS.ParseDouble, SplitFromGoogleSheets(item), defDouble, mustBeAllNumbers);
            sb.AppendLine(NHCalculateMedianAverage(list));
        }
        return sb.ToString();
    }
    public static string CalculateMedianFromTwoRows(string s, Func<List<double>, string> NHCalculateMedianAverage)
    {
        var r = Rows(s);
        for (var i = 0; i < r.Count; i++) r[i] = CalculateMedianAverage(r[i], true, NHCalculateMedianAverage);
        return string.Join(Environment.NewLine, r);
    }
    public static List<List<string>> AllLines(string d)
    {
        var result = new List<List<string>>();
        var l = SHGetLines.GetLines(d);
        foreach (var item in l) result.Add(GetRowCells(item));
        return result;
    }

    public static List<string> GetRowCells(string ClipboardS)
    {
        return SplitFromGoogleSheets(ClipboardS);
    }

    /// <summary>
    ///     If null, will be  load from clipboard
    /// </summary>
    /// <param name="input"></param>
    public static List<string> Rows(string input)
    {
        //if (input == null)
        //{
        //    input = ClipboardHelper.GetText();
        //}
        return input.Split('\n').ToList(); //SHSplit.Split(input, "\n");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="input"></param>
    /// <param name=""></param>
    /// <returns></returns>
    public static List<string> SplitFromGoogleSheetsRow(string input /*, bool removeEmptyElementsFromEnd*/)
    {
        //if (input == null)
        //{
        //    input = ClipboardHelper.GetText();
        //}
        var r = SplitFromGoogleSheets(input);
        bool removeEmptyElementsFromEnd = true;
        if (removeEmptyElementsFromEnd)
        {
            for (var i = r.Count - (1); i >= 0; i--)
            {
                if (string.IsNullOrWhiteSpace(r[i]))
                {
                    r.RemoveAt(i);
                }
                else
                {
                    break;
                }
            }
            //r = r.Where(d => !string.IsNullOrWhiteSpace(d)).ToList();
        }

        //CA.RemoveStringsEmpty2(r);
        return r;
    }

    public static List<string> SplitFromGoogleSheets2(string input)
    {
        return SHGetLines.GetLines(input);
    }

    /// <summary>
    ///     rozděluje podle tab / space, pokud to chci podle \r\n tak SplitFromGoogleSheets2
    ///     If A1 null, take from clipboard
    ///     Not to parse whole content
    /// </summary>
    /// <param name="input"></param>
    public static List<string> SplitFromGoogleSheets(string input)
    {
        var bm = SH.TabOrSpaceNextTo(input);
        //List<string> vr = new List<string>();
        //if (bm.Count > 0)
        //{
        //    vr.AddRange(SHSplit.SplitByIndexes(input, bm));
        //    vr.Reverse();
        //}
        //else
        //{
        //    //ThisApp.Warning( "Bad data in clipboard");
        //    vr.Add(input);
        //}
        var vr2 = SHSplit.SplitNone(input, "\t");
        return vr2;
    }
    /// <summary>
    ///     A1 are column names for ValuesTableGrid (not letter sorted a,b,.. but left column (Name, Rating, etc.)
    ///     A2 are data
    /// </summary>
    /// <param name="captions_FirstColumn"></param>
    /// <param name="exists_OtherColumn"></param>
    public static string SwitchForGoogleSheets(List<string> captions_FirstColumn, List<List<string>> exists_OtherColumn, bool throwExIfDifferentCountOfCaptionsAndExists = false)
    {
        var countFirst = captions_FirstColumn.Count;
        Dictionary<int, List<int>> columnsWithDifferentElementsList = new();
        List<int> withRightCount = new();
        for (int i = 0; i < exists_OtherColumn.Count; i++)
        {
            var count = exists_OtherColumn[i].Count;
            if (count != countFirst)
            {
                if (columnsWithDifferentElementsList.ContainsKey(count))
                {
                    columnsWithDifferentElementsList[count].Add(i);
                }
                else
                {
                    columnsWithDifferentElementsList.Add(count, [i]);
                }
            }
            else
            {
                withRightCount.Add(i);
            }
        }
        StringBuilder sb = new();
        if (columnsWithDifferentElementsList.Count != 0)
        {
            if (throwExIfDifferentCountOfCaptionsAndExists)
            {
                sb.AppendLine($"Different count in captions {columnsWithDifferentElementsList.Count} and exists:");
                sb.AppendLine("Count in column - Columns list");
                foreach (var item in columnsWithDifferentElementsList)
                {
                    sb.AppendLine(item.Key + " - " + string.Join(",", item.Value));
                }
                ThrowEx.Custom(sb.ToString());
            }
            else
            {
                var max = columnsWithDifferentElementsList.Keys.Max();
                if (max > countFirst)
                {
                    for (int i = max - countFirst - 1; i >= 0; i--)
                    {
                        captions_FirstColumn.Add(i.ToString());
                    }
                    countFirst = captions_FirstColumn.Count;
                }
                for (int i = 0; i < exists_OtherColumn.Count; i++)
                {
                    FillUpToSize(exists_OtherColumn[i], countFirst);
                }
            }
        }
        var vtg = new ValuesTableGrid<string>(exists_OtherColumn);
        vtg.captions = captions_FirstColumn;
        var dt = vtg.SwitchRowsAndColumn();
        sb.Clear();
        foreach (DataRow item in dt.Rows) JoinForGoogleSheetRow(sb, item.ItemArray);
        var vr = sb.ToString();
        return vr;
    }
    private static void FillUpToSize(List<string> list, int countFirst)
    {
        var to = countFirst - list.Count;
        for (int i = 0; i < to; i++)
        {
            list.Add(i.ToString());
        }
    }
    /// <summary>
    ///     Dříve byl IList ale ten nemůže být protože string.Join nemá takové přetížení. vrátí potom místo spojeného stringu
    ///     System.Object[]
    /// </summary>
    /// <param name="sb"></param>
    /// <param name="en"></param>
    public static void JoinForGoogleSheetRow(StringBuilder sb, object[] en)
    {
        sb.AppendLine(JoinForGoogleSheetRow(en));
    }
    public static string JoinForGoogleSheetRow(object[] en)
    {
        var r = string.Join('\t', en);
        return r;
    }
    /// <summary>
    ///     Snad to bude fungovat
    ///     JoinForGoogleSheetRow(item.ItemArray) stále odkazuje na JoinForGoogleSheetRow(Object[] en)
    ///     Jinde v app ale používám List<string> a proto potřebuji i toto
    /// </summary>
    /// <param name="en"></param>
    /// <returns></returns>
    public static string JoinForGoogleSheetRow(IEnumerable<string> en)
    {
        var r = string.Join('\t', en);
        return r;
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