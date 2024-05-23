
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunamoGoogleSheets;
public class SH
{
    public static List<int> TabOrSpaceNextTo(string s)
    {
        List<int> nt = new List<int>();

        nt.AddRange(ReturnOccurencesOfString(s, "\t"));
        nt.AddRange(ReturnOccurencesOfString(s, " "));

        nt.Sort();

        return nt;
    }

    public static string NormalizeString(string s)
    {
        if (s.Contains(AllChars.nbsp))
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in s)
            {
                if (item == AllChars.nbsp)
                {
                    sb.Append(AllChars.space);
                }
                else
                {
                    sb.Append(item);
                }
            }
            return sb.ToString();
        }

        return s;
    }

    public static List<int> ReturnOccurencesOfString(string vcem, string co)
    {
        vcem = NormalizeString(vcem);
        List<int> Results = new List<int>();
        for (int Index = 0; Index < (vcem.Length - co.Length) + 1; Index++)
        {
            var subs = vcem.Substring(Index, co.Length);
            ////////DebugLogger.Instance.WriteLine(subs);
            // non-breaking space. &nbsp; code 160
            // 32 space
            char ch = subs[0];
            char ch2 = co[0];
            if (subs == AllStrings.space)
            {
            }
            if (subs == co)
                Results.Add(Index);
        }
        return Results;
    }
}
