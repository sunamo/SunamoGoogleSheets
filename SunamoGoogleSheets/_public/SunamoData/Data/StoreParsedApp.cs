namespace SunamoGoogleSheets._public.SunamoData.Data;

/// <summary>
/// Stores parsed application data from Google Sheets
/// </summary>
public class StoreParsedApp
{
    /// <summary>Field name for application name</summary>
    public const string Name = "Name";
    /// <summary>Field name for application category</summary>
    public const string Category = "Category";
    /// <summary>Field name for application URI</summary>
    public const string Uri = "Uri";
    /// <summary>Field name for count of ratings</summary>
    public const string CountOfRatings = "Count of ratings";
    /// <summary>Field name for average rating</summary>
    public const string AverageRating = "Average rating";
    /// <summary>Field name for overall users in thousands</summary>
    public const string OverallUsersInThousandsK = "Overall users in thousands (k)";
    /// <summary>Field name for price</summary>
    public const string Price = "Price";
    /// <summary>Field name for in-app purchases</summary>
    public const string InAppPurchases = "In-app purchases";
    /// <summary>Field name for last updated date</summary>
    public const string LastUpdated = "Last updated";
    /// <summary>Field name for run test status</summary>
    public const string RunTest = "Run test";
    /// <summary>Field name for final official web</summary>
    public const string FinalOfficialWeb = "Final - Official Web";
    /// <summary>Field name for further test status</summary>
    public const string FurtherTest = "Further test";
    /// <summary>Field name for price for year subscription</summary>
    public const string PriceForYearSubs = "Price for year subs";
    /// <summary>Field name for price for lifelong subscription</summary>
    public const string PriceForLifelongSubs = "Price for lifelong subs";

    /// <summary>
    /// Gets or sets the application name
    /// </summary>
    public string? AppName { get; set; } = null;

    /// <summary>
    /// Gets or sets the application URI
    /// </summary>
    public string? AppUri { get; set; } = null;

    /// <summary>
    /// Gets the value for a specific field/column
    /// </summary>
    /// <param name="fieldName">The name of the field/column</param>
    /// <returns>The value for the specified field, or empty string if not found</returns>
    public string GetValueForRow(string fieldName)
    {
        switch (fieldName)
        {
            case Name:
                return AppName;
            case Uri:
                return AppUri;
            default:
                return string.Empty;
        }
    }
}