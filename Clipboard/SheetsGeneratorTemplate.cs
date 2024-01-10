namespace SunamoGoogleSheets.Clipboard;

public class SheetsGeneratorTemplate
{



    public static string AndroidAppComparing(List<StoreParsedApp> spa)
    {
        Dictionary<string, List<object>> ob = new Dictionary<string, List<object>>();

        var l = @"Name
Category
Uri
Count of ratings
Average rating
Overall users in thousands (k)
Price
In-app purchases
Last updated

Run test

Final - Official Web
Further test
Price for year subs
Price for lifelong subs";

        DataTable dt = new DataTable();
        dt.Columns.Add();
        foreach (var item in spa)
        {
            dt.Columns.Add();
        }
        var li = SHGetLines.GetLines(l);

        foreach (var item in li)
        {
            List<object> lo = new List<object>(spa.Count + 1);

            lo.Add(item);

            foreach (var item2 in spa)
            {
                lo.Add(item2.GetValueForRow(item));
            }

            if (item != string.Empty)
            {
                ob.Add(item, lo);
            }
        }

        foreach (var item in li)
        {
            var row = dt.NewRow();

            if (item != string.Empty)
            {
                row.ItemArray = ob[item].ToArray();
            }
            else
            {
                row.ItemArray = new Object[] { string.Empty };
            }
            dt.Rows.Add(row);
        }

        return SheetsHelper.DataTableToString(dt);
    }
}
