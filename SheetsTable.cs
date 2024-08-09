namespace SunamoGoogleSheets;

public class SheetsTable
{
    private readonly DataTable dt = new();
    public Dictionary<string, FromToTGoogleSheets<int>> ft = new();

    public SheetsTable(string input)
    {
        var r = SheetsHelper.Rows(input);

        var maxColumns = 0;

        foreach (var item in r)
        {
            var c = SheetsHelper.SplitFromGoogleSheetsRow(item, false);
            if (c.Count > maxColumns) maxColumns = c.Count;
        }

        var columnNames = SheetsHelper.SplitFromGoogleSheetsRow(r.First(), false);
        foreach (var item in columnNames) dt.Columns.Add(item);

        var i = 0;

        foreach (var item in r)
        {
            var c = SheetsHelper.SplitFromGoogleSheetsRow(item, false);
            var dr = dt.NewRow();


            dr.ItemArray = c.ConvertAll(d => (object)d).ToArray();

            var first = c.First();

            if (c.Skip(1).ToList().All(d => d == string.Empty) && first.EndsWith(":"))
            {
                if (ft.Count > 0) ft.Last().Value.to = i;

                var ft2 = new FromToTGoogleSheets<int>();
                ft2.from = i + 1;

                ft.Add(first, ft2);
            }


            dt.Rows.Add(dr);

            i++;
        }

        if (ft.Count > 0) ft.Last().Value.to = i;
    }

    public List<string> RowsFromColumn(int dx, FromToTGoogleSheets<int> ft = null)
    {
        var vr = new List<string>();

        if (ft != null)
            for (var i = ft.from; i < ft.to; i++)
                vr.Add(dt.Rows[i].ItemArray[dx].ToString());
        else
            foreach (DataRow item in dt.Rows)
                vr.Add(item.ItemArray[dx].ToString());

        return vr;
    }
}