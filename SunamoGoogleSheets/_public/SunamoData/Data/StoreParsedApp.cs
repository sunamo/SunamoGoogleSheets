namespace SunamoGoogleSheets._public.SunamoData.Data;

/// <summary>
/// Stores parsed application data from Google Sheets
/// </summary>
public class StoreParsedApp
{
    public const string Name = "Name";
    public const string Category = "Category";
    public const string Uri = "Uri";
    public const string CountOfRatings = "Count of ratings";
    public const string AverageRating = "Average rating";
    public const string OverallUsersInThousandsK = "Overall users in thousands (k)";
    public const string Price = "Price";
    public const string InAppPurchases = "In-app purchases";
    public const string LastUpdated = "Last updated";
    public const string RunTest = "Run test";
    public const string FinalOfficialWeb = "Final - Official Web";
    public const string FurtherTest = "Further test";
    public const string PriceForYearSubs = "Price for year subs";
    public const string PriceForLifelongSubs = "Price for lifelong subs";

    /// <summary>
    /// Gets or sets the application name
    /// </summary>
    public string AppName { get; set; } = null;

    /// <summary>
    /// Gets or sets the application URI
    /// </summary>
    public string AppUri { get; set; } = null;

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