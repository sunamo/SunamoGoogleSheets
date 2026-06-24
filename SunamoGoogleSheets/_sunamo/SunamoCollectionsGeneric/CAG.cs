namespace SunamoGoogleSheets._sunamo.SunamoCollectionsGeneric;

internal class CAG
{
    internal static int LowestCount<T>(List<List<T>> lists)
    {
        var min = int.MaxValue;

        foreach (var item in lists)
            if (min > item.Count)
                min = item.Count;

        return min;
    }

    internal static List<List<T>> TrimInnersToCount<T>(List<List<T>> lists, int count)
    {
        for (var i = 0; i < lists.Count; i++) lists[i] = lists[i].Take(count).ToList();

        return lists;
    }
}
