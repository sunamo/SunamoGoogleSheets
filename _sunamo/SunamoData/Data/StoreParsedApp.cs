namespace SunamoGoogleSheets;


public class StoreParsedApp
{
    internal string name = null;
    internal string uri = null;
    internal const string Name = "Name";
    internal const string Category = "Category";
    internal const string Uri = "Uri";
    internal const string CountOfRatings = "Count of ratings";
    internal const string AverageRating = "Average rating";
    internal const string OverallUsersInThousandsK = "Overall users in thousands (k)";
    internal const string Price = "Price";
    internal const string InAppPurchases = "In-app purchases";
    internal const string LastUpdated = "Last updated";
    internal const string RunTest = "Run test";
    internal const string FinalOfficialWeb = "Final - Official Web";
    internal const string FurtherTest = "Further test";
    internal const string PriceForYearSubs = "Price for year subs";
    internal const string PriceForLifelongSubs = "Price for lifelong subs";
    internal string GetValueForRow(string fc)
    {
        switch (fc)
        {
            case Name:
                return name;
            case Uri:
                return uri;
            default:
                return string.Empty;
        }
    }
}