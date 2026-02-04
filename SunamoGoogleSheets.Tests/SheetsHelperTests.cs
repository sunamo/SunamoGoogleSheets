// variables names: ok
using System.Data;

namespace SunamoGoogleSheets.Tests;

/// <summary>
/// Tests for SheetsHelper class
/// </summary>
public class SheetsHelperTests
{
    /// <summary>
    /// Tests DataTableToString method
    /// </summary>
    [Fact]
    public void DataTableToStringTest()
    {
        DataTable dataTable = new();
        dataTable.Columns.Add("Column");
        dataTable.Columns.Add("Column 2");

        dataTable.Rows.Add(["conventional-changelog-cli", "@semantic-release/release-notes-generator"]);
        dataTable.Rows.Add(["https://www.npmjs.com/package/conventional-changelog-cli", " https://www.npmjs.com/package/@semantic-release/release-notes-generator"]);

        var result = SheetsHelper.DataTableToString(dataTable);

    }

    /// <summary>
    /// Tests SwitchForGoogleSheets method
    /// </summary>
    [Fact]
    public void SwitchForGoogleSheetsTest()
    {
        var result = SheetsHelper.SwitchForGoogleSheets(["a", "b"], new List<List<string>>([["conventional-changelog-cli", "https://www.npmjs.com/package/conventional-changelog-cli"], ["@semantic-release/release-notes-generator", " https://www.npmjs.com/package/@semantic-release/release-notes-generator"]]));

        var simpleResult = SheetsHelper.SwitchForGoogleSheets(["a", "b"], new List<List<string>>([["c", "d"], ["e", "f"]]));
        ClipboardService.SetText(result);
    }

    /// <summary>
    /// Tests SwitchRowsAndColumn method
    /// </summary>
    [Fact]
    public void SwitchRowsAndColumnTest()
    {
        //        ClipboardService.SetText(SheetsHelper.SwitchRowsAndColumn(@"Package	SunamoDevCode	SunamoDelegates
        //Across unique .cs files (without _sunamo and _public)	37	30"));
        var testData = @"Package	SunamoDevCode	SunamoDelegates	SunamoRoslyn	SunamoExtensions	SunamoXliffParser	SunamoAttributes	SunamoShared	SunamoWinStd	SunamoHttp	SunamoCl	SunamoFileSystem	SunamoPS	SunamoTextOutputGenerator	SunamoEditorConfig	SunamoValues	SunamoCollections	SunamoCsv	SunamoParsing	SunamoReflection	SunamoRuleset	SunamoUnderscore	SunamoNumbers	SunamoString	SunamoVcf	SunamoCollectionWithoutDuplicates	SunamoCompare	SunamoCsproj	SunamoCssGenerator	SunamoDebugIO	SunamoDictionary	SunamoExceptions	SunamoFileExtensions	SunamoFileIO	SunamoRandom	SunamoUriWebServices	SunamoXml	SunamoArgs	SunamoBts	SunamoCollectionOnDrive	SunamoCollectionsGeneric	SunamoCrypt	SunamoDebugging	SunamoGitConfig	SunamoHtml	SunamoLogMessage	SunamoMail	SunamoPackageJson	SunamoRegex	SunamoToUnixLineEnding	SunamoBazosCrawler	SunamoColors	SunamoDateTime	SunamoDebugCollection	SunamoLang	SunamoPlatformUwpInterop	SunamoStopwatch	SunamoText	SunamoThisApp	SunamoUri	SunamoAsync	SunamoChar	SunamoClearScript	SunamoClipboard	SunamoCollectionsChangeContent	SunamoCollectionsIndexesWithNull	SunamoCollectionsTo	SunamoCollectionsValuesTableGrid	SunamoDotnetCmdBuilder	SunamoEmbeddedResources	SunamoEnumsHelper	SunamoGetFiles	SunamoGetFolders	SunamoGoogleSheets	SunamoIni	SunamoJson	SunamoLaTeX	SunamoLogging	SunamoMarkdown	SunamoMathpix	SunamoMime	SunamoMsgReader	SunamoNuGetProtocol	SunamoOctokit	SunamoPercentCalculator	SunamoRobotsTxt	SunamoRss	SunamoSerializer	SunamoStringData	SunamoStringFormat	SunamoStringGetLines	SunamoStringGetString	SunamoStringJoin	SunamoStringJoinPairs	SunamoStringParts	SunamoStringReplace	SunamoStringSplit	SunamoStringSubstring	SunamoStringTrim	SunamoThread	SunamoTwoWayDictionary	SunamoWikipedia	SunamoXlfKeys	SunamoYaml	SunamoYouTube	SunamoCollectionsNonGeneric	SunamoConverters	SunamoData	SunamoEnums	SunamoFluentFtp	SunamoFtp	SunamoInterfaces	SunamoPInvoke	SunamoSolutionsIndexer	SunamoTidy
Across unique .cs files (without _sunamo and _public)	37	30	16	15	15	13	11	11	10	9	9	9	9	8	8	7	7	7	7	7	7	6	6	6	5	5	5	5	5	5	5	5	5	5	5	5	4	4	4	4	4	4	4	4	4	4	4	4	4	3	3	3	3	3	3	3	3	3	3	2	2	2	2	2	2	2	2	2	2	2	2	2	2	2	2	2	2	2	2	2	2	2	2	2	2	2	2	2	2	2	2	2	2	2	2	2	2	2	2	2	2	2	2	2	1	1	1	1	1	1	1	1	1	1
Code lines	1942	82	1146	288	534	44	220	1024	458	667	1325	337	348	112	351	313	318	190	452	312	57	298	1813	142	189	167	488	287	165	387	49	888	565	185	447	465	24	543	148	240	372	46	106	322	46	199	119	90	14	29	37	85	39	64	33	98	45	52	424	214	183	26	50	77	32	9	67	20	38	162	321	105	67	46	18	2416	53	25	41	27	35	35	57	52	77	63	239	57	57	45	33	109	22	58	347	377	48	115	23	25	60	5174	14	63	11	20	19	13	18	26	21	18	12	12
Documentation lines	235	0	152	27	53	5	10	140	70	105	217	27	48	0	39	95	135	45	32	7	14	45	272	0	13	7	62	5	8	59	4	22	19	29	36	65	4	74	2	71	92	4	0	46	8	40	5	15	0	0	2	4	2	0	0	18	5	7	56	16	8	0	3	47	4	0	7	0	11	39	24	13	0	12	0	0	7	4	4	0	0	6	0	7	0	0	50	4	49	0	0	26	0	12	64	47	20	9	0	0	7	3	0	8	0	0	0	0	0	0	0	0	0	0
Test code lines	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0
";
        ClipboardService.SetText(SheetsHelper.SwitchRowsAndColumn(testData));
    }

    /// <summary>
    /// Tests SplitFromGoogleSheets method with simple data
    /// </summary>
    [Fact]
    public void SplitFromGoogleSheetsRowTest()
    {
        var input = "a\tbc\td";
        var actual = SheetsHelper.SplitFromGoogleSheets(input);
        Assert.Equal(["a", "bc", "d"], actual);
    }

    /// <summary>
    /// Tests SplitFromGoogleSheets method with empty cells
    /// </summary>
    [Fact]
    public void SplitFromGoogleSheetsRowTest2()
    {
        var input = "a\tbc\t\td";
        var actual = SheetsHelper.SplitFromGoogleSheets(input);
        Assert.Equal(["a", "bc", "d"], actual);
    }
}