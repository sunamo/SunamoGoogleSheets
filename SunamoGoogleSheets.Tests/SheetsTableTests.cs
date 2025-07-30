namespace SunamoGoogleSheets.Tests;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

public class SheetsTableTests
{
    ILogger logger = NullLogger.Instance;
    SheetsTable sheetsTable = new SheetsTable(NullLogger.Instance);
    const string defFile = @"D:\_Test\RealityIdnesCz.Cmd\RemoveAlreadyDeleted.txt";

    async Task ParseRowsWithDefaultFile()
    {
        sheetsTable.ParseRows(await TF.ReadAllText(defFile));
    }

    [Fact]
    public async Task ParseRowsTest()
    {
        await ParseRowsWithDefaultFile();

    }

    [Fact]
    public async Task RowsFromColumnTest()
    {
        await ParseRowsWithDefaultFile();
        var rows = sheetsTable.RowsFromColumn(1);
    }

    [Fact]
    public async Task ParseRowsOfSectionsTest()
    {
        var st = new SheetsTable(logger);
        st.ParseRowsOfSections(await TF.ReadAllText(defFile));

        var d = st.RowsFromColumn(0);
        //var ftf = st.ft.First();
        //foreach (var item in st.RowsFromColumn(0, ftf.Value))
        //{
        //}
    }

    [Fact]
    public async Task DeleteColumnTest()
    {
        await ParseRowsWithDefaultFile();
        sheetsTable.DeleteColumn(1);
        var d = SheetsHelper.DataTableToString(sheetsTable.Table);
        ClipboardService.SetText(d);
    }
}