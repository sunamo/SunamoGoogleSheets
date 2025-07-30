namespace RunnerGoogleSheets;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SunamoCl.SunamoCmd;
using SunamoCl.SunamoCmd.Args;
using SunamoGoogleSheets.Tests;

partial class Program
{

    const string appName = "";

    static ServiceCollection Services = new();
    static ServiceProvider Provider;
    static ILogger logger;

    static Program()
    {
        CmdBootStrap.AddILogger(Services, true, null, appName);



        Provider = Services.BuildServiceProvider();

        logger = Provider.GetService<ILogger>() ?? throw new ServiceNotFoundException(nameof(ILogger));
    }

    static void Main()
    {
        MainAsync().GetAwaiter().GetResult();
    }

    static async Task MainAsync()
    {
        var runnedAction = await CmdBootStrap.RunWithRunArgs(new RunArgs
        {
            AddGroupOfActions = AddGroupOfActions,
            Args = [],
            AskUserIfRelease = true,
            RunInDebugAsync = RunInDebugAsync,
            ServiceCollection = Services,
            IsDebug =
#if DEBUG
            true
#else
false
#endif
        });



        //Console.WriteLine("Finished: " + runnedAction);
        Console.ReadLine();
    }

    static async Task RunInDebugAsync()
    {
        await Task.Delay(1);

        SheetsHelperTests t = new SheetsHelperTests();
        //t.SwitchForGoogleSheetsTest();
        //t.SwitchRowsAndColumnTest();
        //t.DataTableToStringTest();
        t.SwitchForGoogleSheetsTest();

        //SheetsTableTests t = new();
        //await t.ParseRowsTest();
        //await t.RowsFromColumnTest();
        //await t.DeleteColumnTest();

    }
}