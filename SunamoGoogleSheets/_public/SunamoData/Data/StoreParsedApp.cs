namespace SunamoGoogleSheets._public.SunamoData.Data;

/// <summary>
/// Represents the store parsed app type.
/// </summary>
public class StoreParsedApp
{
    /// <summary>
    /// The name constant.
    /// </summary>
    public const string Name = "Name";
    /// <summary>
    /// The category constant.
    /// </summary>
    public const string Category = "Category";
    /// <summary>
    /// The uri constant.
    /// </summary>
    public const string Uri = "Uri";
    /// <summary>
    /// The count of ratings constant.
    /// </summary>
    public const string CountOfRatings = "Count of ratings";
    /// <summary>
    /// The average rating constant.
    /// </summary>
    public const string AverageRating = "Average rating";
    /// <summary>
    /// The overall users in thousands k constant.
    /// </summary>
    public const string OverallUsersInThousandsK = "Overall users in thousands (k)";
    /// <summary>
    /// The price constant.
    /// </summary>
    public const string Price = "Price";
    /// <summary>
    /// The in app purchases constant.
    /// </summary>
    public const string InAppPurchases = "In-app purchases";
    /// <summary>
    /// The last updated constant.
    /// </summary>
    public const string LastUpdated = "Last updated";
    /// <summary>
    /// The run test constant.
    /// </summary>
    public const string RunTest = "Run test";
    /// <summary>
    /// The final official web constant.
    /// </summary>
    public const string FinalOfficialWeb = "Final - Official Web";
    /// <summary>
    /// The further test constant.
    /// </summary>
    public const string FurtherTest = "Further test";
    /// <summary>
    /// The price for year subs constant.
    /// </summary>
    public const string PriceForYearSubs = "Price for year subs";
    /// <summary>
    /// The price for lifelong subs constant.
    /// </summary>
    public const string PriceForLifelongSubs = "Price for lifelong subs";

    /// <summary>
    /// Gets or sets the app name.
    /// </summary>
    public string? AppName { get; set; } = null;

    /// <summary>
    /// Gets or sets the app uri.
    /// </summary>
    public string? AppUri { get; set; } = null;

    /// <summary>
    /// Get value for row.
    /// </summary>
    /// <param name="k">The k.</param>
    /// <returns>The get value for row result.</returns>
    public string GetValueForRow(string fieldName)
    {
        switch (fieldName)
        {
            case Name:
                return AppName ?? string.Empty;
            case Uri:
                return AppUri ?? string.Empty;
            default:
                return string.Empty;
        }
    }
}
