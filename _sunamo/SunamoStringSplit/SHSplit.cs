using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunamoGoogleSheets;
internal class SHSplit
{
    internal static List<string> Split(string p, params string[] newLine)
    {
        return p.Split(newLine, StringSplitOptions.RemoveEmptyEntries).ToList();
    }
    internal static List<string> SplitByIndexes(string input, List<int> bm)
    {
        List<string> d = new List<string>(bm.Count + 1);
        bm.Sort();
        string before, after;
        before = input;

        for (int i = bm.Count - 1; i >= 0; i--)
        {
            (before, after) = GetPartsByLocationNoOutInt(before, bm[i]);
            d.Insert(0, after);
        }

        (before, after) = GetPartsByLocationNoOutInt(before, bm[0]);
        d.Insert(0, before);
        d.Reverse();
        return d;
    }

    internal static (string, string) GetPartsByLocationNoOutInt(string text, int pozice)
    {
        string pred, za;
        GetPartsByLocation(out pred, out za, text, pozice);
        return (pred, za);
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
            {
                za = text.Substring(pozice + 1);
            }
            else
            {
                za = string.Empty;
            }
        }
    }
}
