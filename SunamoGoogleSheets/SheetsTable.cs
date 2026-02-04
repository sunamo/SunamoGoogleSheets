namespace SunamoGoogleSheets;

/// <summary>
/// Represents a table parsed from Google Sheets with support for sections
/// </summary>
public class SheetsTable(ILogger logger)
{
    /// <summary>
    /// Gets the parsed data table
    /// </summary>
    public DataTable Table { get; private set; } = new();

    /// <summary>
    /// Gets the section ranges (from-to) for each section identified by section name
    /// </summary>
    public Dictionary<string, FromToTGoogleSheets<int>> SectionRanges { get; private set; } = new();

    /// <summary>
    /// Gets the number of columns in the table
    /// </summary>
    public int ColumnCount => Table.Columns.Count;

    /// <summary>
    /// Gets the number of rows in the table
    /// </summary>
    public int RowsCount => Table.Rows.Count;

    /// <summary>
    /// Deletes a column from the table at the specified index
    /// </summary>
    /// <param name="columnIndex">The zero-based index of the column to delete</param>
    public void DeleteColumn(int columnIndex)
    {
        Table.Columns.RemoveAt(columnIndex);
    }

    /// <summary>
    /// Parses rows from Google Sheets clipboard format into the table
    /// </summary>
    /// <param name="input">The text content from Google Sheets clipboard</param>
    public void ParseRows(string input)
    {
        var result = SheetsHelper.Rows(input);

        if (result.Count == 0)
        {
            logger.LogWarning("Google sheet with zero rows was parsed");
        }

        var maxColumnsLength = result.Max(row => SheetsHelper.SplitFromGoogleSheetsRow(row).Count);

        var firstRow = result.First();
        var headerCells = SheetsHelper.SplitFromGoogleSheetsRow(firstRow);

        for (int i = headerCells.Count - 1; i>=0; i--)
        {
            if (string.IsNullOrWhiteSpace(headerCells[i]))
            {
                headerCells.RemoveAt(i);
            }
        }

        var columnCount = headerCells.Count;

        foreach (var item in headerCells)
        {
            Table.Columns.Add(item, typeof(string));
        }

        if (headerCells.Count < maxColumnsLength)
        {
            for (int i = headerCells.Count -1; i < maxColumnsLength; i++)
            {
                Table.Columns.Add("Dummy column " + i);
            }
        }

        foreach (var item in result)
        {
            var cellValues = SheetsHelper.SplitFromGoogleSheetsRow(item);
            var dataRow = Table.NewRow();

            // Column count should match, but trim if needed
            cellValues = cellValues.Take(columnCount).ToList();

            dataRow.ItemArray = cellValues.ConvertAll(cellValue => (object)cellValue).ToArray();

            var first = cellValues.FirstOrDefault();

            if (first == null)
            {
                // When copying from Google Sheets, it's common to copy 1000 rows but most of them are empty
                continue;
            }

            Table.Rows.Add(dataRow);
        }
    }

    /// <summary>
    /// Parses rows and identifies sections (sections are identified by a colon at the end of the first cell)
    /// Returns rows that belong to each section
    /// </summary>
    /// <param name="input">The text content from Google Sheets clipboard</param>
    public void ParseRowsOfSections(string input)
    {
        var result = SheetsHelper.Rows(input);

        var maxColumns = 0;

        foreach (var item in result)
        {
            var cellValues = SheetsHelper.SplitFromGoogleSheetsRow(item);
            if (cellValues.Count > maxColumns) maxColumns = cellValues.Count;
        }

        var columnNames = SheetsHelper.SplitFromGoogleSheetsRow(result.First());
        foreach (var item in columnNames) Table.Columns.Add(item);

        var rowIndex = 0;

        foreach (var item in result)
        {
            var cellValues = SheetsHelper.SplitFromGoogleSheetsRow(item);
            var dataRow = Table.NewRow();


            dataRow.ItemArray = cellValues.ConvertAll(cellValue => (object)cellValue).ToArray();

            var first = cellValues.FirstOrDefault();

            if (first == null)
            {
                // When copying from Google Sheets, it's common to copy 1000 rows but most of them are empty
                continue;

            }

            if (cellValues.Skip(1).ToList().All(cellValue => cellValue == string.Empty) && first.EndsWith(":"))
            {
                if (SectionRanges.Count > 0) SectionRanges.Last().Value.To = rowIndex;

                var sectionRange = new FromToTGoogleSheets<int>();
                sectionRange.From = rowIndex + 1;

                SectionRanges.Add(first, sectionRange);
            }


            Table.Rows.Add(dataRow);

            rowIndex++;
        }

        if (SectionRanges.Count > 0) SectionRanges.Last().Value.To = rowIndex;
    }

    /// <summary>
    /// Returns values from a specific column, optionally filtered to a specific section
    /// </summary>
    /// <param name="columnIndex">The zero-based index of the column to retrieve values from</param>
    /// <param name="sectionRange">Optional section range to filter rows; if null, returns all rows</param>
    /// <returns>List of string values from the specified column</returns>
    public List<string> RowsFromColumn(int columnIndex, FromToTGoogleSheets<int>? sectionRange = null)
    {
        var result = new List<string>();

        if (sectionRange != null)
            for (var i = sectionRange.From; i < sectionRange.To; i++)
                result.Add(Table.Rows[i].ItemArray[columnIndex]?.ToString() ?? string.Empty);
        else
            foreach (DataRow item in Table.Rows)
                result.Add(item.ItemArray[columnIndex]?.ToString() ?? string.Empty);

        return result;
    }
}