namespace SunamoGoogleSheets._sunamo;
internal class SHGetLines
{
    internal static List<string> GetLines(string s)
    {
        return s.Split(new String[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
    }
}
