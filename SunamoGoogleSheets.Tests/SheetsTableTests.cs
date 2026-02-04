// variables names: ok
namespace SunamoGoogleSheets.Tests;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

/// <summary>
/// Tests for SheetsTable class
/// </summary>
public class SheetsTableTests
{
    ILogger logger = NullLogger.Instance;
    SheetsTable sheetsTable = new SheetsTable(NullLogger.Instance);
    const string defaultFile = @"D:\_Test\RealityIdnesCz.Cmd\RemoveAlreadyDeleted.txt";

    async Task ParseRowsWithDefaultFile()
    {
        sheetsTable.ParseRows(await TF.ReadAllText(defaultFile));
    }

    /// <summary>
    /// Tests ParseRows method
    /// </summary>
    [Fact]
    public async Task ParseRowsTest()
    {
        await ParseRowsWithDefaultFile();

    }

    /// <summary>
    /// Tests RowsFromColumn method
    /// </summary>
    [Fact]
    public async Task RowsFromColumnTest()
    {
        await ParseRowsWithDefaultFile();
        var rows = sheetsTable.RowsFromColumn(1);
    }

    /// <summary>
    /// Tests ParseRowsOfSections method
    /// </summary>
    [Fact]
    public async Task ParseRowsOfSectionsTest()
    {
        var table = new SheetsTable(logger);
        table.ParseRowsOfSections(await TF.ReadAllText(defaultFile));

        var data = table.RowsFromColumn(0);
    }

    /// <summary>
    /// Tests DeleteColumn method
    /// </summary>
    [Fact]
    public async Task DeleteColumnTest()
    {
        await ParseRowsWithDefaultFile();
        sheetsTable.DeleteColumn(1);
        var data = SheetsHelper.DataTableToString(sheetsTable.Table);
        ClipboardService.SetText(data);
    }
}