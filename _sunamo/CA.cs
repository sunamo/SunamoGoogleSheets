namespace SunamoGoogleSheets._sunamo;

internal class CA
{
    internal static bool IsListStringWrappedInArray<T>(List<T> v2)
    {
        var first = v2.First().ToString();
        if (v2.Count == 1 && (first == "System.Collections.Generic.List`1[System.String]" ||
                              first == "System.Collections.Generic.List`1[System.Object]")) return true;
        return false;
    }
}