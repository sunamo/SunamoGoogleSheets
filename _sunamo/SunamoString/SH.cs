namespace SunamoGoogleSheets._sunamo.SunamoString;

internal class SH
{
    internal static string JoinNL(List<string> l)
    {
        StringBuilder sb = new();
        foreach (var item in l) sb.AppendLine(item);
        var r = string.Empty;
        r = sb.ToString();
        return r;
    }

    internal static List<string> SplitCharMore(string s, params char[] dot)
    {
        return s.Split(dot, StringSplitOptions.RemoveEmptyEntries).ToList();
    }

    internal static List<string> SplitMore(string s, params string[] dot)
    {
        return s.Split(dot, StringSplitOptions.RemoveEmptyEntries).ToList();
    }

    internal static List<string> SplitNone(string text, params string[] deli)
    {
        return text.Split(deli, StringSplitOptions.None).ToList();
    }

    internal static string NullToStringOrDefault(object n)
    {
        return n == null ? " " + "(null)" : "" + n;
    }

    internal static string TrimEnd(string name, string ext)
    {
        while (name.EndsWith(ext)) return name.Substring(0, name.Length - ext.Length);
        return name;
    }

    internal static List<int> TabOrSpaceNextTo(string s)
    {
        var nt = new List<int>();

        nt.AddRange(ReturnOccurencesOfString(s, "\t"));
        nt.AddRange(ReturnOccurencesOfString(s, " "));

        nt.Sort();

        return nt;
    }

    internal static string NormalizeString(string s)
    {
        if (s.Contains((char)160))
        {
            var sb = new StringBuilder();
            foreach (var item in s)
                if (item == (char)160)
                    sb.Append(' ');
                else
                    sb.Append(item);
            return sb.ToString();
        }

        return s;
    }

    internal static List<int> ReturnOccurencesOfString(string vcem, string co)
    {
        var Results = new List<int>();
        for (var Index = 0; Index < vcem.Length - co.Length + 1; Index++)
        {
            var subs = vcem.Substring(Index, co.Length);
            ////////DebugLogger.Instance.WriteLine(subs);
            // non-breaking space. &nbsp; code 160
            // 32 space
            var ch = subs[0];
            var ch2 = co[0];
            if (subs == "")
            {
            }

            if (subs == co)
                Results.Add(Index);
        }

        return Results;
    }
}