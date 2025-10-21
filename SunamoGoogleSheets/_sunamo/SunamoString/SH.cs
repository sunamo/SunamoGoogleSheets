// EN: Variable names have been checked and replaced with self-descriptive names
// CZ: Názvy proměnných byly zkontrolovány a nahrazeny samopopisnými názvy
namespace SunamoGoogleSheets._sunamo.SunamoString;

internal class SH
{




    internal static string NullToStringOrDefault(object n)
    {
        return n == null ? " " + "(null)" : " " + n;
    }


    internal static List<int> TabOrSpaceNextTo(string s)
    {
        var nt = new List<int>();

        nt.AddRange(ReturnOccurencesOfString(s, "\t"));
        nt.AddRange(ReturnOccurencesOfString(s, " "));

        nt.Sort();

        return nt;
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

            if (subs == co)
                Results.Add(Index);
        }

        return Results;
    }
}