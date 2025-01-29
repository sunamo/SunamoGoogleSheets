namespace SunamoGoogleSheets._sunamo.SunamoStringSplit;

internal class SHSplit
{
    internal static List<string> SplitMore(string p, params string[] newLine)
    {
        return p.Split(newLine, StringSplitOptions.RemoveEmptyEntries).ToList();
    }



    internal static void GetPartsByLocation(out string pred, out string za, string text, int pozice)
    {
        if (pozice == -1)
        {
            pred = text;
            za = "";
        }
        else
        {
            pred = text.Substring(0, pozice);
            if (text.Length > pozice + 1)
                za = text.Substring(pozice + 1);
            else
                za = string.Empty;
        }
    }
}