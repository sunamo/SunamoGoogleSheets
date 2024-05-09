namespace SunamoGoogleSheets._sunamo;
internal class SHGetLines
{
    internal static List<string> GetLines(string s)
    {
        return sv.Split(new string[] { v.Contains("\r\n") ? "\r\n" : "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
    }
}
