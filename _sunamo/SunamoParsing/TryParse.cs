namespace SunamoGoogleSheets;


public class TryParse
{
    public class DateTime
    {
        // was moved to E:\vs\Projects\sunamoWithoutLocalDep\SunamoDateTime\DT\DTHelperMulti.cs
    }
    public class Integer
    {
        public static Integer Instance = new Integer();
        public int lastInt = -1;
        /// <summary>
        /// Vrátí True pokud se podaří vyparsovat, jinak false.
        /// Výsledek najdeš v proměnné lastInt
        /// </summary>
        /// <param name="p"></param>
        public bool TryParseInt(string p)
        {
            if (int.TryParse(p, out lastInt))
            {
                return true;
            }
            return false;
        }
    }
}