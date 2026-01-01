namespace SunamoGoogleSheets.Clipboard;

/// <summary>
/// Helper class for parsing and formatting data from Google Sheets clipboard format
/// </summary>
public class SheetsHelper
{
    /// <summary>
    /// Gets the first letter from a Google Sheets cell if it is followed by a space
    /// </summary>
    /// <param name="item2">The cell content to examine</param>
    /// <returns>The first character if followed by a space, otherwise null</returns>
    public static char? FirstLetterFromSheet(string item2)
    {
        if (item2.Length > 2)
            if (item2[1] == ' ')
                return item2[0];
        return null;
    }

    /// <summary>
    /// Switches rows and columns in tabular text data (transposes the data)
    /// </summary>
    /// <param name="text">The text containing tabular data with rows and columns</param>
    /// <param name="keepInSizeOfSmallest">If true, keeps only columns up to the size of the smallest row</param>
    /// <returns>The transposed data as text</returns>
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
        temp.Captions = GetRowCells(list[0]);
        var dt = temp.SwitchRowsAndColumn();
        return DataTableToString(dt);
    }

    /// <summary>
    /// Converts a DataTable to Google Sheets formatted text (tab-delimited)
    /// </summary>
    /// <param name="text">The DataTable to convert</param>
    /// <returns>Tab-delimited text representation of the DataTable</returns>
    public static string DataTableToString(DataTable text)
    {
        var stringBuilder = new StringBuilder();
        foreach (DataRow item in text.Rows) stringBuilder.AppendLine(JoinForGoogleSheetRow(item.ItemArray));
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
    /// <param name="NHCalculateMedianAverage">Function to calculate median or average from a list of numbers</param>
    /// <returns>Text with calculated values for each row</returns>
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

    /// <summary>
    /// Calculates median or average for data organized in two rows
    /// </summary>
    /// <param name="text">The text containing two rows of data</param>
    /// <param name="NHCalculateMedianAverage">Function to calculate median or average from a list of numbers</param>
    /// <returns>Text with calculated values</returns>
    public static string CalculateMedianFromTwoRows(string text, Func<List<double>, string> NHCalculateMedianAverage)
    {
        var result = Rows(text);
        for (var i = 0; i < result.Count; i++) result[i] = CalculateMedianAverage(result[i], true, NHCalculateMedianAverage);
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
    /// <param name="captions_FirstColumn">Column names (not letter sorted like A,B,C but actual names like Name, Rating, etc.)</param>
    /// <param name="exists_OtherColumn">Data columns</param>
    /// <param name="throwExIfDifferentCountOfCaptionsAndExists">If true, throws exception when caption count differs from data count</param>
    /// <returns>Transposed data formatted for Google Sheets</returns>
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
        vtg.Captions = captions_FirstColumn;
        var dt = vtg.SwitchRowsAndColumn();
        stringBuilder.Clear();
        foreach (DataRow item in dt.Rows) JoinForGoogleSheetRow(stringBuilder, item.ItemArray);
        var result = stringBuilder.ToString();
        return result;
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
    /// Joins array elements with tab delimiter and appends to StringBuilder (previously was IList but string.Join doesn't support that overload)
    /// </summary>
    /// <param name="stringBuilder">StringBuilder to append the result to</param>
    /// <param name="en">Array of objects to join</param>
    public static void JoinForGoogleSheetRow(StringBuilder stringBuilder, object[] en)
    {
        stringBuilder.AppendLine(JoinForGoogleSheetRow(en));
    }

    /// <summary>
    /// Joins array elements with tab delimiter for Google Sheets row format
    /// </summary>
    /// <param name="en">Array of objects to join</param>
    /// <returns>Tab-delimited string</returns>
    public static string JoinForGoogleSheetRow(object[] en)
    {
        var result = string.Join('\t', en);
        return result;
    }
    /// <summary>
    /// Joins string enumerable with tab delimiter for Google Sheets row format (overload for List&lt;string&gt; and other IEnumerable types)
    /// </summary>
    /// <param name="en">String enumerable to join</param>
    /// <returns>Tab-delimited string</returns>
    public static string JoinForGoogleSheetRow(IEnumerable<string> en)
    {
        var result = string.Join('\t', en);
        return result;
    }
}