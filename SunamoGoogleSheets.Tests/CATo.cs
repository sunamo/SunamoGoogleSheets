// variables names: ok
namespace SunamoGoogleSheets.Tests;

/// <summary>
/// Provides collection conversion utilities for tests
/// </summary>
public class CATo
{
    /// <summary>
    /// Converts an array to a list
    /// </summary>
    public static List<T> To<T>(params T[] items)
    {
        return items.ToList();
    }
}