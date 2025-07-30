namespace SunamoGoogleSheets.Tests;

public class CATo
{
    public static List<T> To<T>(params T[] t)
    {
        return t.ToList();
    }
}