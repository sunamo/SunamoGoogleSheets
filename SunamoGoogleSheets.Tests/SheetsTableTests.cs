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
        var table = new SheetsTable(logger);
        table.ParseRowsOfSections(await TF.ReadAllText(defFile));

        var data = table.RowsFromColumn(0);
    }

    [Fact]
    public async Task DeleteColumnTest()
    {
        await ParseRowsWithDefaultFile();
        sheetsTable.DeleteColumn(1);
        var data = SheetsHelper.DataTableToString(sheetsTable.Table);
        ClipboardService.SetText(data);
    }
}