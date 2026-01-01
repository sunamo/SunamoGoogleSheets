namespace SunamoGoogleSheets._sunamo.SunamoCollectionsGeneric;

/// <summary>
/// Provides generic collection manipulation utilities
/// </summary>
internal class CAG
{
    /// <summary>
    /// Finds the lowest count among all inner lists
    /// </summary>
    /// <typeparam name="T">The type of elements in the lists</typeparam>
    /// <param name="lists">The list of lists to examine</param>
    /// <returns>The smallest count found among all inner lists</returns>
    internal static int LowestCount<T>(List<List<T>> lists)
    {
        var min = int.MaxValue;

        foreach (var item in lists)
            if (min > item.Count)
                min = item.Count;

        return min;
    }

    /// <summary>
    /// Trims all inner lists to the specified count
    /// </summary>
    /// <typeparam name="T">The type of elements in the lists</typeparam>
    /// <param name="lists">The list of lists to trim</param>
    /// <param name="count">The maximum count to keep in each inner list</param>
    /// <returns>The trimmed list of lists</returns>
    internal static List<List<T>> TrimInnersToCount<T>(List<List<T>> lists, int count)
    {
        for (var i = 0; i < lists.Count; i++) lists[i] = lists[i].Take(count).ToList();

        return lists;
    }
}