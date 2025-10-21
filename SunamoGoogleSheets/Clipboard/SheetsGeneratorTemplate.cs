// EN: Variable names have been checked and replaced with self-descriptive names
// CZ: Názvy proměnných byly zkontrolovány a nahrazeny samopopisnými názvy

namespace SunamoGoogleSheets.Clipboard;

public class SheetsGeneratorTemplate
{
    public static string AndroidAppComparing(List<StoreParsedApp> spa)
    {
        var ob = new Dictionary<string, List<object>>();

        var list = @"Name
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

        var dt = new DataTable();
        dt.Columns.Add();
        foreach (var item in spa) dt.Columns.Add();
        var li = SHGetLines.GetLines(list);

        foreach (var item in li)
        {
            var lo = new List<object>(spa.Count + 1);

            lo.Add(item);

            foreach (var item2 in spa) lo.Add(item2.GetValueForRow(item));

            if (item != string.Empty) ob.Add(item, lo);
        }

        foreach (var item in li)
        {
            var row = dt.NewRow();

            if (item != string.Empty)
                row.ItemArray = ob[item].ToArray();
            else
                row.ItemArray = new object[] { string.Empty };
            dt.Rows.Add(row);
        }

        return SheetsHelper.DataTableToString(dt);
    }
}