



namespace SunamoGoogleSheets;
//namespace SunamoGoogleSheets;


public class SheetsTable
{
    DataTable dt = new DataTable();
    public Dictionary<string, FromToTGoogleSheets<int>> ft = new Dictionary<string, FromToTGoogleSheets<int>>();
    public SheetsTable(string input)
    {
        var r = SheetsHelper.Rows(input);

        int maxColumns = 0;

        foreach (var item in r)
        {
            var c = SheetsHelper.SplitFromGoogleSheetsRow(item, false);
            if (c.Count > maxColumns)
            {
                maxColumns = c.Count;
            }
        }

        var columnNames = SheetsHelper.SplitFromGoogleSheetsRow(r.First<string>(), false);
        foreach (var item in columnNames)
        {
            dt.Columns.Add(item);
        }

        int i = 0;

        foreach (var item in r)
        {
            var c = SheetsHelper.SplitFromGoogleSheetsRow(item, false);
            var dr = dt.NewRow();



            dr.ItemArray = c.ConvertAll(d => (object)d).ToArray();

            var first = c.First<string>();

            if (c.Skip(1).ToList().All(d => d == string.Empty) && first.EndsWith(":"))
            {
                if (ft.Count > 0)
                {
                    ft.Last().Value.to = i;
                }

                var ft2 = new FromToTGoogleSheets<int>();
                ft2.from = i + 1;

                ft.Add(first, ft2);
            }



            dt.Rows.Add(dr);

            i++;
        }

        if (ft.Count > 0)
        {
            ft.Last().Value.to = i;
        }
    }

    public List<string> RowsFromColumn(int dx, FromToTGoogleSheets<int> ft = null)
    {
        List<string> vr = new List<string>();

        if (ft != null)
        {
            for (int i = ft.from; i < ft.to; i++)
            {
                vr.Add(dt.Rows[i].ItemArray[dx].ToString());
            }
        }
        else
        {
            foreach (DataRow item in dt.Rows)
            {
                vr.Add(item.ItemArray[dx].ToString());
            }
        }

        return vr;
    }
}
