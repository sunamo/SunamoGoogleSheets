namespace SunamoGoogleSheets;
public class BTS
{
    public static double ParseDouble(string entry, double _default)
    {
        //entry = SHSH.FromSpace160To32(entry);
        entry = entry.Replace(" ", string.Empty);
        //var ch = entry[3];

        double lastDouble2 = 0;
        if (double.TryParse(entry, out lastDouble2))
        {
            return lastDouble2;
        }
        return _default;
    }
}
