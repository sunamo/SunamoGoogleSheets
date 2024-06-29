namespace SunamoGoogleSheets;


public class FromToTSHGoogleSheets<T>
{

    internal bool empty;
    protected long fromL;
    internal FromToUseGoogleSheets ftUse = FromToUseGoogleSheets.DateTime;
    protected long toL;
    internal FromToTSHGoogleSheets()
    {
        var t = typeof(T);
        if (t == Types.tInt) ftUse = FromToUseGoogleSheets.None;
    }
    /// <summary>
    ///     Use Empty contstant outside of class
    /// </summary>
    /// <param name="empty"></param>
    private FromToTSHGoogleSheets(bool empty) : this()
    {
        this.empty = empty;
    }
    /// <summary>
    ///     A3 true = DateTime
    ///     A3 False = None
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="ftUse"></param>
    internal FromToTSHGoogleSheets(T from, T to, FromToUseGoogleSheets ftUse = FromToUseGoogleSheets.DateTime) : this()
    {
        this.from = from;
        this.to = to;
        this.ftUse = ftUse;
    }
    internal T from
    {
        get => (T)(dynamic)fromL;
        set => fromL = (long)(dynamic)value;
    }
    internal T to
    {
        get => (T)(dynamic)toL;
        set => toL = (long)(dynamic)value;
    }
    internal long FromL => fromL;
    internal long ToL => toL;
}