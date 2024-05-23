namespace SunamoGoogleSheets;
public class SHGetLines
{
    public static List<string> GetLines(string s)
    {
        return s.Split(new string[] { s.Contains("\r\n") ? "\r\n" : "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
    }
}
