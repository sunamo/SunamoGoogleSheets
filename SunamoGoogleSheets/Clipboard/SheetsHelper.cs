namespace SunamoGoogleSheets.Clipboard;

/// <summary>
/// Helper class for parsing and formatting data from Google Sheets clipboard format
/// </summary>
public class SheetsHelper
{
    /// <summary>
    /// Gets the first letter from a Google Sheets cell if it is followed by a space
    /// </summary>
    /// <param name="cellContent">The cell content to examine</param>
    /// <returns>The first character if followed by a space, otherwise null</returns>
    public static char? FirstLetterFromSheet(string cellContent)
    {
        if (cellContent.Length > 2)
            if (cellContent[1] == ' ')
                return cellContent[0];
        return null;
    }

    /// <summary>
    /// Switches rows and columns in tabular text data (transposes the data)
    /// </summary>
    /// <param name="text">The text containing tabular data with rows and columns</param>
    /// <param name="isKeepingInSizeOfSmallest">If true, keeps only columns up to the size of the smallest row</param>
    /// <returns>The transposed data as text</returns>
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

    /// <summary>
    /// Converts a DataTable to Google Sheets formatted text (tab-delimited)
    /// </summary>
    /// <param name="dataTable">The DataTable to convert</param>
    /// <returns>Tab-delimited text representation of the DataTable</returns>
    public static string DataTableToString(DataTable dataTable)
    {
        var stringBuilder = new StringBuilder();
        foreach (DataRow item in dataTable.Rows) stringBuilder.AppendLine(JoinForGoogleSheetRow(item.ItemArray!));
        return stringBuilder.ToString();
    }

    /// <summary>
    /// Generates Excel/Google Sheets style column identifiers (A, B, C, ..., Z, AA, AB, AC, ...)
    /// </summary>
    /// <param name="count">The number of column identifiers to generate</param>
    /// <returns>List of column identifiers</returns>
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
    /// Generates column name in Excel/Google Sheets style (A, B, C, ..., Z, AA, AB, AC, ...)
    /// </summary>
    /// <param name="columnIndex">Column index (0-based)</param>
    /// <returns>Column name</returns>
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
    /// Calculates median or average for each row in the input text
    /// </summary>
    /// <param name="input">The text containing rows of numbers</param>
    /// <param name="isRequiringAllNumbers">If true, all values must be valid numbers</param>
    /// <param name="calculateFunction">Function to calculate median or average from a list of numbers</param>
    /// <returns>Text with calculated values for each row</returns>
    public static string CalculateMedianAverage(string input, bool isRequiringAllNumbers,
        Func<List<double>, string> calculateFunction)
    {
        var sourceList = Rows(input);
        var stringBuilder = new StringBuilder();
        foreach (var item in sourceList)
        {
            var defaultValue = -1.0;
            var list = CAToNumber.ToNumber<double>(BTS.ParseDouble, SplitFromGoogleSheets(item), defaultValue, isRequiringAllNumbers);
            stringBuilder.AppendLine(calculateFunction(list));
        }
        return stringBuilder.ToString();
    }

    /// <summary>
    /// Calculates median or average for data organized in two rows
    /// </summary>
    /// <param name="text">The text containing two rows of data</param>
    /// <param name="calculateFunction">Function to calculate median or average from a list of numbers</param>
    /// <returns>Text with calculated values</returns>
    public static string CalculateMedianFromTwoRows(string text, Func<List<double>, string> calculateFunction)
    {
        var result = Rows(text);
        for (var i = 0; i < result.Count; i++) result[i] = CalculateMedianAverage(result[i], true, calculateFunction);
        return string.Join(Environment.NewLine, result);
    }

    /// <summary>
    /// Parses all lines from text into a 2D list of cells
    /// </summary>
    /// <param name="text">The text containing tabular data</param>
    /// <returns>2D list where each inner list represents cells in a row</returns>
    public static List<List<string>> AllLines(string text)
    {
        var result = new List<List<string>>();
        var list = SHGetLines.GetLines(text);
        foreach (var item in list) result.Add(GetRowCells(item));
        return result;
    }

    /// <summary>
    /// Splits a row from Google Sheets into individual cell values
    /// </summary>
    /// <param name="clipboardText">The row text from clipboard</param>
    /// <returns>List of cell values</returns>
    public static List<string> GetRowCells(string clipboardText)
    {
        return SplitFromGoogleSheets(clipboardText);
    }

    /// <summary>
    /// Splits the input text into rows by newline character
    /// </summary>
    /// <param name="input">The text to split into rows</param>
    /// <returns>List of row strings</returns>
    public static List<string> Rows(string input)
    {
        return input.Split('\n').ToList();
    }

    /// <summary>
    /// Splits a Google Sheets row into cells and removes empty elements from the end
    /// </summary>
    /// <param name="input">The row text to split</param>
    /// <returns>List of cell values with trailing empty elements removed</returns>
    public static List<string> SplitFromGoogleSheetsRow(string input)
    {
        var result = SplitFromGoogleSheets(input);

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

    /// <summary>
    /// Splits text by line breaks (alternative method that splits by \r\n instead of tabs)
    /// </summary>
    /// <param name="input">The text to split</param>
    /// <returns>List of lines</returns>
    public static List<string> SplitFromGoogleSheets2(string input)
    {
        return SHGetLines.GetLines(input);
    }

    /// <summary>
    /// Splits text by tab or space characters (for splitting by \r\n use SplitFromGoogleSheets2)
    /// </summary>
    /// <param name="input">The text to split</param>
    /// <returns>List of cell values</returns>
    public static List<string> SplitFromGoogleSheets(string input)
    {
        var result = SHSplit.SplitNone(input, "\t");
        return result;
    }
    /// <summary>
    /// Switches rows and columns for Google Sheets format, using first column as captions
    /// </summary>
    /// <param name="captions">Column names (not letter sorted like A,B,C but actual names like Name, Rating, etc.)</param>
    /// <param name="dataColumns">Data columns</param>
    /// <param name="isThrowingExceptionIfDifferentCountOfCaptionsAndExists">If true, throws exception when caption count differs from data count</param>
    /// <returns>Transposed data formatted for Google Sheets</returns>
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

    /// <summary>
    /// Fills a list with string representations of indices until it reaches the target size
    /// </summary>
    /// <param name="list">The list to fill</param>
    /// <param name="targetSize">The target size to reach</param>
    private static void FillUpToSize(List<string> list, int targetSize)
    {
        var elementsToAdd = targetSize - list.Count;
        for (int i = 0; i < elementsToAdd; i++)
        {
            list.Add(i.ToString());
        }
    }

    /// <summary>
    /// Joins array elements with tab delimiter and appends to StringBuilder (previously was IList but string.Join doesn't support that overload)
    /// </summary>
    /// <param name="stringBuilder">StringBuilder to append the result to</param>
    /// <param name="cells">Array of objects to join</param>
    public static void JoinForGoogleSheetRow(StringBuilder stringBuilder, object[] cells)
    {
        stringBuilder.AppendLine(JoinForGoogleSheetRow(cells));
    }

    /// <summary>
    /// Joins array elements with tab delimiter for Google Sheets row format
    /// EN: CRITICAL - Sanitizes cell values by removing newlines, tabs, and carriage returns that would break TSV format
    /// CZ: KRITICKÉ - Sanitizuje hodnoty buněk odstraněním newline, tab a carriage return znaků které by rozbily TSV formát
    /// </summary>
    /// <param name="cells">Array of objects to join</param>
    /// <returns>Tab-delimited string</returns>
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

        var result = string.Join('\t', sanitizedCells);
        return result;
    }

    /// <summary>
    /// Joins string enumerable with tab delimiter for Google Sheets row format (overload for List&lt;string&gt; and other IEnumerable types)
    /// EN: CRITICAL - Sanitizes cell values by removing newlines, tabs, and carriage returns that would break TSV format
    /// CZ: KRITICKÉ - Sanitizuje hodnoty buněk odstraněním newline, tab a carriage return znaků které by rozbily TSV formát
    /// </summary>
    /// <param name="cells">String enumerable to join</param>
    /// <returns>Tab-delimited string</returns>
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

        var result = string.Join('\t', sanitizedCells);
        return result;
    }
}