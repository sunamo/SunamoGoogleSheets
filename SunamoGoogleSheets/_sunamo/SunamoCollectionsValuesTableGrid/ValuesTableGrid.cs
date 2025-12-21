namespace SunamoGoogleSheets._sunamo.SunamoCollectionsValuesTableGrid;

/// <summary>
///     Similar class with two dimension array is UniqueTableInWhole
///     Allow make query to parallel collections as be one
/// </summary>
/// <typeparam name="T"></typeparam>
internal class ValuesTableGrid<T> : List<List<T>> //, IValuesTableGrid<T>
{
    /// <summary>
    ///     Row - wrapper - files 2
    ///     Column - inner - apps 4
    /// </summary>
    private readonly List<List<T>> _exists;

    internal List<string> captions;

    internal ValuesTableGrid(List<List<T>> exists, bool keepInSizeOfSmallest = true)
    {
        if (keepInSizeOfSmallest)
        {
            var lowest = CAG.LowestCount(exists);
            exists = CAG.TrimInnersToCount(exists, lowest);
        }

        _exists = exists;
    }

    /// <summary>
    ///     Must be initialized captions variable
    ///     All rows must be trimmed from \r \n
    /// </summary>
    internal DataTable SwitchRowsAndColumn()
    {
        var newTable = new DataTable();
        if (_exists.Count > 0)
        {
            // Prvně přidám prázdný sloupec kde budou captions
            newTable.Columns.Add(string.Empty);
            // Můžu přidám sloupec pro B,C,D...
            for (var i = 0; i < _exists.Count; i++)
                newTable.Columns.Add();
            var text = _exists[0];
            for (var i = 0; i < text.Count; i++)
            {
                var newRow = newTable.NewRow();
                var caption = captions[i]; //CA.GetIndex(captions, i);
                newRow[0] = caption == null ? string.Empty : caption;
                for (var j = 0; j < _exists.Count; j++)
                    newRow[j + 1] = _exists[j][i];
                newTable.Rows.Add(newRow);
            }
        }

        return newTable;
    }



}