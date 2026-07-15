namespace SunamoGoogleSheets._sunamo.SunamoCollectionsValuesTableGrid;

// Row - wrapper - files 2
// Column - inner - apps 4
internal class ValuesTableGrid<T> : List<List<T>>
{
    private readonly List<List<T>> data;

    internal List<string>? Captions { get; set; }

    internal ValuesTableGrid(List<List<T>> lists, bool isKeepingInSizeOfSmallest = true)
    {
        if (isKeepingInSizeOfSmallest)
        {
            var lowest = CAG.LowestCount(lists);
            lists = CAG.TrimInnersToCount(lists, lowest);
        }

        data = lists;
    }

    // Captions must be initialized before calling this method
    // All rows must be trimmed from \r \n characters
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
                newRow[0] = caption ?? string.Empty;
                for (var j = 0; j < data.Count; j++)
                    newRow[j + 1] = data[j][i];
                newTable.Rows.Add(newRow);
            }
        }

        return newTable;
    }
}
