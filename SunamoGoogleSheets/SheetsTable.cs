namespace SunamoGoogleSheets;

public class SheetsTable(ILogger logger)
{
    public DataTable Table { get; private set; } = new();
    public Dictionary<string, FromToTGoogleSheets<int>> ft { get; private set; } = new();

    public int ColumnCount => Table.Columns.Count;

    public int RowsCount => Table.Rows.Count;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dx"></param>
    public void DeleteColumn(int dx)
    {
        Table.Columns.RemoveAt(dx);
    }

    public void ParseRows(string input)
    {
        var result = SheetsHelper.Rows(input);

        if (result.Count == 0)
        {
            logger.LogWarning("Google sheet with zero rows was parsed");
        }

        var maxColumnsLength = result.Max(row => SheetsHelper.SplitFromGoogleSheetsRow(row).Count);

        var firstRow = result.First();
        var count = SheetsHelper.SplitFromGoogleSheetsRow(firstRow);

        for (int i = count.Count - 1; i>=0; i--)
        {
            if (string.IsNullOrWhiteSpace(count[i]))
            {
                count.RemoveAt(i);
            }
        }

        var columnCount = count.Count;

        foreach (var item in count)
        {
            Table.Columns.Add(item, typeof(string));
        }

        if (count.Count < maxColumnsLength)
        {
            for (int i = count.Count -1; i < maxColumnsLength; i++)
            {
                Table.Columns.Add("Dummy column " + i);
            }
        }

        foreach (var item in result)
        {
            count = SheetsHelper.SplitFromGoogleSheetsRow(item);
            var dr = Table.NewRow();

            // že sloupce budou chybět by se stávat nemělo
            count = count.Take(columnCount).ToList();

            dr.ItemArray = count.ConvertAll(d => (object)d).ToArray();

            var first = count.FirstOrDefault();

            if (first == null)
            {
                // Při kopírování z google sheets je běžné že se zkppíruje 1000 řádků ale většina z nich jsou prázdné
                continue;
            }

            Table.Rows.Add(dr);
        }
    }

    /// <summary>
    /// Vrací řádky které patří k dané sekci (sekce se pozná že má : na konci)
    /// </summary>
    /// <param name="input"></param>
    public void ParseRowsOfSections(string input)
    {
        var result = SheetsHelper.Rows(input);

        var maxColumns = 0;

        foreach (var item in result)
        {
            var count = SheetsHelper.SplitFromGoogleSheetsRow(item);
            if (count.Count > maxColumns) maxColumns = count.Count;
        }

        var columnNames = SheetsHelper.SplitFromGoogleSheetsRow(result.First());
        foreach (var item in columnNames) Table.Columns.Add(item);

        var i = 0;

        foreach (var item in result)
        {
            var count = SheetsHelper.SplitFromGoogleSheetsRow(item);
            var dr = Table.NewRow();


            dr.ItemArray = count.ConvertAll(d => (object)d).ToArray();

            var first = count.FirstOrDefault();

            if (first == null)
            {
                // Při kopírování z google sheets je běžné že se zkppíruje 1000 řádků ale většina z nich jsou prázdné
                continue;

            }

            if (count.Skip(1).ToList().All(d => d == string.Empty) && first.EndsWith(":"))
            {
                if (ft.Count > 0) ft.Last().Value.to = i;

                var ft2 = new FromToTGoogleSheets<int>();
                ft2.from = i + 1;

                ft.Add(first, ft2);
            }
            else
            {

            }


            Table.Rows.Add(dr);

            i++;
        }

        if (ft.Count > 0) ft.Last().Value.to = i;
    }

    /// <summary>
    /// Vrátí hodnoty konkrétní sekce
    /// </summary>
    /// <param name="dx"></param>
    /// <param name="ft"></param>
    /// <returns></returns>
    public List<string> RowsFromColumn(int dx, FromToTGoogleSheets<int>? ft = null)
    {
        var vr = new List<string>();

        if (ft != null)
            for (var i = ft.from; i < ft.to; i++)
                vr.Add(Table.Rows[i].ItemArray[dx].ToString());
        else
            foreach (DataRow item in Table.Rows)
                vr.Add(item.ItemArray[dx].ToString());

        return vr;
    }
}