namespace SunamoGoogleSheets.Clipboard;

public class SheetsHelper
{
    public static char? FirstLetterFromSheet(string cellContent)
    {
        if (cellContent.Length > 2)
            if (cellContent[1] == ' ')
                return cellContent[0];
        return null;
    }

    public static string SwitchRowsAndColumn(string text, bool isKeepingInSizeOfSmallest = true)
    {
        var exists = new List<List<string>>();
        var list = SHGetLines.GetLines(text);
        foreach (var item in list.Skip(1))
        {
            if (item.Trim() == "") continue;
            exists.Add(GetRowCells(item));
        }
        var tableGrid = new ValuesTableGrid<string>(exists, isKeepingInSizeOfSmallest);
        tableGrid.Captions = GetRowCells(list[0]);
        var dataTable = tableGrid.SwitchRowsAndColumn();
        return DataTableToString(dataTable);
    }

    public static string DataTableToString(DataTable dataTable)
    {
        var stringBuilder = new StringBuilder();
        foreach (DataRow item in dataTable.Rows) stringBuilder.AppendLine(JoinForGoogleSheetRow(item.ItemArray!));
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

    public static string CalculateMedianAverage(string text, bool isRequiringAllNumbers,
        Func<List<double>, string> calculateFunction)
    {
        var sourceList = Rows(text);
        var stringBuilder = new StringBuilder();
        foreach (var item in sourceList)
        {
            var defaultValue = -1.0;
            var list = CAToNumber.ToNumber<double>(BTS.ParseDouble, SplitFromGoogleSheets(item), defaultValue, isRequiringAllNumbers);
            stringBuilder.AppendLine(calculateFunction(list));
        }
        return stringBuilder.ToString();
    }

    public static string CalculateMedianFromTwoRows(string text, Func<List<double>, string> calculateFunction)
    {
        var result = Rows(text);
        for (var i = 0; i < result.Count; i++) result[i] = CalculateMedianAverage(result[i], true, calculateFunction);
        return string.Join(Environment.NewLine, result);
    }

    public static List<List<string>> AllLines(string text)
    {
        var result = new List<List<string>>();
        var list = SHGetLines.GetLines(text);
        foreach (var item in list) result.Add(GetRowCells(item));
        return result;
    }

    public static List<string> GetRowCells(string text)
    {
        return SplitFromGoogleSheets(text);
    }

    public static List<string> Rows(string text)
    {
        return text.Split('\n').ToList();
    }

    public static List<string> SplitFromGoogleSheetsRow(string text)
    {
        var result = SplitFromGoogleSheets(text);

        for (var i = result.Count - 1; i >= 0; i--)
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

        return result;
    }

    public static List<string> SplitFromGoogleSheets2(string text)
    {
        return SHGetLines.GetLines(text);
    }

    public static List<string> SplitFromGoogleSheets(string text)
    {
        var result = SHSplit.SplitNone(text, "\t");
        return result;
    }

    public static string SwitchForGoogleSheets(List<string> captions, List<List<string>> dataColumns, bool isThrowingExceptionIfDifferentCountOfCaptionsAndExists = false)
    {
        var captionCount = captions.Count;
        Dictionary<int, List<int>> columnsWithDifferentElementsList = new();
        List<int> columnsWithCorrectCount = new();
        for (int i = 0; i < dataColumns.Count; i++)
        {
            var columnCount = dataColumns[i].Count;
            if (columnCount != captionCount)
            {
                if (columnsWithDifferentElementsList.ContainsKey(columnCount))
                {
                    columnsWithDifferentElementsList[columnCount].Add(i);
                }
                else
                {
                    columnsWithDifferentElementsList.Add(columnCount, [i]);
                }
            }
            else
            {
                columnsWithCorrectCount.Add(i);
            }
        }
        StringBuilder stringBuilder = new();
        if (columnsWithDifferentElementsList.Count != 0)
        {
            if (isThrowingExceptionIfDifferentCountOfCaptionsAndExists)
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
                if (max > captionCount)
                {
                    for (int i = max - captionCount - 1; i >= 0; i--)
                    {
                        captions.Add(i.ToString());
                    }
                    captionCount = captions.Count;
                }
                for (int i = 0; i < dataColumns.Count; i++)
                {
                    FillUpToSize(dataColumns[i], captionCount);
                }
            }
        }
        var tableGrid = new ValuesTableGrid<string>(dataColumns);
        tableGrid.Captions = captions;
        var dataTable = tableGrid.SwitchRowsAndColumn();
        stringBuilder.Clear();
        foreach (DataRow item in dataTable.Rows) JoinForGoogleSheetRow(stringBuilder, item.ItemArray!);
        var result = stringBuilder.ToString();
        return result;
    }

    private static void FillUpToSize(List<string> list, int targetSize)
    {
        var elementsToAdd = targetSize - list.Count;
        for (int i = 0; i < elementsToAdd; i++)
        {
            list.Add(i.ToString());
        }
    }

    public static void JoinForGoogleSheetRow(StringBuilder stringBuilder, object[] cells)
    {
        stringBuilder.AppendLine(JoinForGoogleSheetRow(cells));
    }

    // EN: CRITICAL - Sanitizes cell values by removing newlines, tabs, and carriage returns that would break TSV format
    // CZ: KRITICKÉ - Sanitizuje hodnoty buněk odstraněním newline, tab a carriage return znaků které by rozbily TSV formát
    public static string JoinForGoogleSheetRow(object[] cells)
    {
        // EN: Sanitize each cell value - remove/replace characters that would break TSV format
        // CZ: Sanitizovat každou hodnotu - odstranit/nahradit znaky které by rozbily TSV formát
        var sanitizedCells = cells.Select(cell =>
        {
            if (cell == null) return string.Empty;
            var str = cell.ToString() ?? string.Empty;
            // EN: Replace newlines, tabs, and carriage returns with spaces to prevent TSV format corruption
            // CZ: Nahradit newline, tab a carriage return znaky mezerami aby se předešlo poškození TSV formátu
            str = str.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\t", " ");
            return str;
        }).ToArray();

        var result = string.Join("\t", sanitizedCells);
        return result;
    }

    // EN: CRITICAL - Sanitizes cell values by removing newlines, tabs, and carriage returns that would break TSV format
    // CZ: KRITICKÉ - Sanitizuje hodnoty buněk odstraněním newline, tab a carriage return znaků které by rozbily TSV formát
    public static string JoinForGoogleSheetRow(IEnumerable<string> cells)
    {
        // EN: Sanitize each cell value - remove/replace characters that would break TSV format
        // CZ: Sanitizovat každou hodnotu - odstranit/nahradit znaky které by rozbily TSV formát
        var sanitizedCells = cells.Select(cell =>
        {
            if (cell == null) return string.Empty;
            // EN: Replace newlines, tabs, and carriage returns with spaces to prevent TSV format corruption
            // CZ: Nahradit newline, tab a carriage return znaky mezerami aby se předešlo poškození TSV formátu
            var str = cell.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\t", " ");
            return str;
        });

        var result = string.Join("\t", sanitizedCells);
        return result;
    }
}
