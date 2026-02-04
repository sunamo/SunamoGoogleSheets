namespace SunamoGoogleSheets._sunamo.SunamoCollectionsValuesTableGrid;

/// <summary>
/// Represents a grid of values organized in a table structure
/// Similar class with two dimension array is UniqueTableInWhole
/// Allows making queries to parallel collections as one
/// </summary>
/// <typeparam name="T">The type of values in the grid</typeparam>
internal class ValuesTableGrid<T> : List<List<T>>
{
    /// <summary>
    /// The underlying data structure
    /// Row - wrapper - files 2
    /// Column - inner - apps 4
    /// </summary>
    private readonly List<List<T>> data;

    /// <summary>
    /// Gets or sets the column captions for the grid
    /// </summary>
    internal List<string>? Captions { get; set; }

    /// <summary>
    /// Initializes a new instance of the ValuesTableGrid class
    /// </summary>
    /// <param name="lists">The list of lists to initialize the grid with</param>
    /// <param name="isKeepingInSizeOfSmallest">If true, trims all inner lists to the size of the smallest list</param>
    internal ValuesTableGrid(List<List<T>> lists, bool isKeepingInSizeOfSmallest = true)
    {
        if (isKeepingInSizeOfSmallest)
        {
            var lowest = CAG.LowestCount(lists);
            lists = CAG.TrimInnersToCount(lists, lowest);
        }

        data = lists;
    }

    /// <summary>
    /// Switches rows and columns, creating a transposed DataTable
    /// Captions must be initialized before calling this method
    /// All rows must be trimmed from \r \n characters
    /// </summary>
    /// <returns>A DataTable with rows and columns switched</returns>
    internal DataTable SwitchRowsAndColumn()
    {
        var newTable = new DataTable();
        if (data.Count > 0)
        {
            // First add an empty column for captions
            newTable.Columns.Add(string.Empty);
            // Then add columns for B,C,D...
            for (var i = 0; i < data.Count; i++)
                newTable.Columns.Add();
            var firstRow = data[0];
            for (var i = 0; i < firstRow.Count; i++)
            {
                var newRow = newTable.NewRow();
                var caption = Captions?[i];
                newRow[0] = caption == null ? string.Empty : caption;
                for (var j = 0; j < data.Count; j++)
                    newRow[j + 1] = data[j][i];
                newTable.Rows.Add(newRow);
            }
        }

        return newTable;
    }
}