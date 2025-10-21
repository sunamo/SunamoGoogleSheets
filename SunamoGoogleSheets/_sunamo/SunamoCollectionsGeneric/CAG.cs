// EN: Variable names have been checked and replaced with self-descriptive names
// CZ: Názvy proměnných byly zkontrolovány a nahrazeny samopopisnými názvy
namespace SunamoGoogleSheets._sunamo.SunamoCollectionsGeneric;

internal class CAG
{
    internal static int LowestCount<T>(List<List<T>> exists)
    {
        var min = int.MaxValue;

        foreach (var item in exists)
            if (min > item.Count)
                min = item.Count;

        return min;
    }



    internal static List<List<T>> TrimInnersToCount<T>(List<List<T>> exists, int lowest)
    {
        for (var i = 0; i < exists.Count; i++) exists[i] = exists[i].Take(lowest).ToList();

        return exists;
    }
}