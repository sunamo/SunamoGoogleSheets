namespace SunamoGoogleSheets._sunamo.SunamoParsing;

internal class TryParse
{
    internal class DateTime
    {
        // was moved to E:\vs\Projects\PlatformIndependentNuGetPackages\SunamoDateTime\DT\DTHelperMulti.cs
    }

    internal class Integer
    {
        internal static Integer Instance = new();
        internal int lastInt = -1;

        /// <summary>
        ///     Vrátí True pokud se podaří vyparsovat, jinak false.
        ///     Výsledek najdeš v proměnné lastInt
        /// </summary>
        /// <param name="p"></param>
        internal bool TryParseInt(string p)
        {
            if (int.TryParse(p, out lastInt)) return true;
            return false;
        }
    }
}