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
    public static string SwitchRowsAndColumn(string text, bool keepInSizeOfSmallest = true)
    {
        var exists = new List<List<string>>();
        var list = SHGetLines.GetLines(text);
        foreach (var item in list.Skip(1))
        {
            if (item.Trim() == "") continue;
            exists.Add(GetRowCells(item));
        }
        var temp = new ValuesTableGrid<string>(exists, keepInSizeOfSmallest);
        temp.captions = GetRowCells(list[0]);
        var dt = temp.SwitchRowsAndColumn();
        return DataTableToString(dt);
    }
    public static string DataTableToString(DataTable text)
    {
        var stringBuilder = new StringBuilder();
        foreach (DataRow item in text.Rows) stringBuilder.AppendLine(JoinForGoogleSheetRow(item.ItemArray));
        return stringBuilder.ToString();
    }
    public static List<string> ColumnsIds(int count)
    {
        var result = new List<string>();
        
        for (int i = 0; i < count; i++)
        {
            result.Add(GetColumnName(i));
        }
        
        return result;
    }
    
    /// <summary>
    /// Generuje názvy sloupců ve stylu Excel/Google Sheets (a, b, c, ..., z, aa, ab, ac, ...)
    /// </summary>
    /// <param name="columnIndex">Index sloupce (0-based)</param>
    /// <returns>Název sloupce</returns>
    private static string GetColumnName(int columnIndex)
    {
        string columnName = string.Empty;
        
        do
        {
            columnName = (char)('A' + (columnIndex % 26)) + columnName;
            columnIndex = (columnIndex / 26) - 1;
        }
        while (columnIndex >= 0);

        return columnName;
    }
    /// <summary>
    ///     A2 was = true
    /// </summary>
    /// <param name="input"></param>
    /// <param name="isRequiringAllNumbers"></param>
    /// <param name="NHCalculateMedianAverage"></param>
    /// <returns></returns>
    public static string CalculateMedianAverage(string input, bool isRequiringAllNumbers,
        Func<List<double>, string> NHCalculateMedianAverage)
    {
        var sourceList = Rows(input);
        var stringBuilder = new StringBuilder();
        foreach (var item in sourceList)
        {
            var defDouble = -1;
            var list = CAToNumber.ToNumber<double>(BTS.ParseDouble, SplitFromGoogleSheets(item), defDouble, isRequiringAllNumbers);
            stringBuilder.AppendLine(NHCalculateMedianAverage(list));
        }
        return stringBuilder.ToString();
    }
    public static string CalculateMedianFromTwoRows(string text, Func<List<double>, string> NHCalculateMedianAverage)
    {
        var result = Rows(text);
        for (var i = 0; i < result.Count; i++) result[i] = CalculateMedianAverage(result[i], true, NHCalculateMedianAverage);
        return string.Join(Environment.NewLine, result);
    }
    public static List<List<string>> AllLines(string d)
    {
        var result = new List<List<string>>();
        var list = SHGetLines.GetLines(d);
        foreach (var item in list) result.Add(GetRowCells(item));
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
        var result = SplitFromGoogleSheets(input);
        bool removeEmptyElementsFromEnd = true;
        if (removeEmptyElementsFromEnd)
        {
            for (var i = result.Count - (1); i >= 0; i--)
            {
                if (string.IsNullOrWhiteSpace(result[i]))
                {
                    result.RemoveAt(i);
                }
                else
                {
                    break;
                }
            }
            //r = result.Where(d => !string.IsNullOrWhiteSpace(d)).ToList();
        }

        //CA.RemoveStringsEmpty2(result);
        return result;
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
        StringBuilder stringBuilder = new();
        if (columnsWithDifferentElementsList.Count != 0)
        {
            if (throwExIfDifferentCountOfCaptionsAndExists)
            {
                stringBuilder.AppendLine($"Different count in captions {columnsWithDifferentElementsList.Count} and exists:");
                stringBuilder.AppendLine("Count in column - Columns list");
                foreach (var item in columnsWithDifferentElementsList)
                {
                    stringBuilder.AppendLine(item.Key + " - " + string.Join(",", item.Value));
                }
                ThrowEx.Custom(stringBuilder.ToString());
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
        stringBuilder.Clear();
        foreach (DataRow item in dt.Rows) JoinForGoogleSheetRow(stringBuilder, item.ItemArray);
        var vr = stringBuilder.ToString();
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
    public static void JoinForGoogleSheetRow(StringBuilder stringBuilder, object[] en)
    {
        stringBuilder.AppendLine(JoinForGoogleSheetRow(en));
    }
    public static string JoinForGoogleSheetRow(object[] en)
    {
        var result = string.Join('\t', en);
        return result;
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
        var result = string.Join('\t', en);
        return result;
    }
    //public static void JoinForGoogleSheetRow(StringBuilder stringBuilder, IList en)
    //{
    //    SheetsHelper.JoinForGoogleSheetRow(stringBuilder, en);
    //}
    //public static string JoinForGoogleSheetRow(IList en)
    //{
    //    return SheetsHelper.JoinForGoogleSheetRow(en);
    //}
}