namespace SunamoGoogleSheets.Clipboard;

/// <summary>
/// Provides templates for generating Google Sheets content
/// </summary>
public class SheetsGeneratorTemplate
{
    /// <summary>
    /// Generates a comparison table for Android apps
    /// </summary>
    /// <param name="parsedApps">The list of parsed app data</param>
    /// <returns>String representation of the comparison table</returns>
    public static string AndroidAppComparing(List<StoreParsedApp> parsedApps)
    {
        var valuesByField = new Dictionary<string, List<object>>();

        var fieldNamesText = @"Name
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

        var dataTable = new DataTable();
        dataTable.Columns.Add();
        foreach (var item in parsedApps) dataTable.Columns.Add();
        var fieldNames = SHGetLines.GetLines(fieldNamesText);

        foreach (var item in fieldNames)
        {
            var rowValues = new List<object>(parsedApps.Count + 1);

            rowValues.Add(item);

            foreach (var app in parsedApps) rowValues.Add(app.GetValueForRow(item));

            if (item != string.Empty) valuesByField.Add(item, rowValues);
        }

        foreach (var item in fieldNames)
        {
            var row = dataTable.NewRow();

            if (item != string.Empty)
                row.ItemArray = valuesByField[item].ToArray();
            else
                row.ItemArray = new object[] { string.Empty };
            dataTable.Rows.Add(row);
        }

        return SheetsHelper.DataTableToString(dataTable);
    }
}