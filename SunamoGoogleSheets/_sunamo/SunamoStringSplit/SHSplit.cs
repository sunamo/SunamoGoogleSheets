namespace SunamoGoogleSheets._sunamo.SunamoStringSplit;

internal class SHSplit
{
    internal static List<string> Split(string p, params string[] newLine)
    {
        return p.Split(newLine, StringSplitOptions.RemoveEmptyEntries).ToList();
    }

    internal static List<string> SplitNone(string text, params string[] deli)
    {
        return text.Split(deli, StringSplitOptions.None).ToList();
    }

}