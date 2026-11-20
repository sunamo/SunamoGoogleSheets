namespace SunamoGoogleSheets;

/// <summary>
/// Sheets table.
/// </summary>
/// <param name="logger">The logger.</param>
/// <returns>The sheets table result.</returns>
public class SheetsTable(ILogger logger)
{
    /// <summary>
    /// Gets or sets the table.
    /// </summary>
    public DataTable Table { get; private set; } = new();

    /// <summary>
    /// Gets or sets the section ranges.
    /// </summary>
    public Dictionary<string, FromToTGoogleSheets<int>> SectionRanges { get; private set; } = new();

    /// <summary>
    /// Gets the column count.
    /// </summary>
    public int ColumnCount => Table.Columns.Count;

    /// <summary>
    /// Gets the rows count.
    /// </summary>
    public int RowsCount => Table.Rows.Count;

    /// <summary>
    /// Delete column.
    /// </summary>
    /// <returns>The delete column result.</returns>
    public void DeleteColumn(int columnIndex)
    {
        Table.Columns.RemoveAt(columnIndex);
    }

    /// <summary>
    /// Parse rows.
    /// </summary>
    /// <param name="columnIndex">The column index.</param>
    /// <returns>The parse rows result.</returns>
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
            cellValues = cellValues.Take(maxColumnsLength).ToList();

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
    /// Parse rows of sections.
    /// </summary>
    /// <returns>The parse rows of sections result.</returns>
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

    /// Returns the cells of one column from top to bottom (optionally only within the section range).
    public List<string> RowsFromColumn(int columnIndex, FromToTGoogleSheets<int>? sectionRange = null)
    {
        // Row indexer, not ItemArray - ItemArray copies the whole row on every read, so calling it in
        // the loop over columns cost rows x columns instead of rows (PerfectHome property card: tens of seconds).
        var result = new List<string>(Table.Rows.Count);

        if (sectionRange != null)
            for (var i = sectionRange.From; i < sectionRange.To; i++)
                result.Add(Table.Rows[i][columnIndex]?.ToString() ?? string.Empty);
        else
            foreach (DataRow item in Table.Rows)
                result.Add(item[columnIndex]?.ToString() ?? string.Empty);

        return result;
    }
}
