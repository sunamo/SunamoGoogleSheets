namespace SunamoGoogleSheets;


public class FromToTSHGoogleSheets<T>
{

    public bool empty;
    protected long fromL;
    public FromToUseGoogleSheets ftUse = FromToUseGoogleSheets.DateTime;
    protected long toL;
    public FromToTSHGoogleSheets()
    {
        var t = typeof(T);
        if (t == Types.tInt) ftUse = FromToUseGoogleSheets.None;
    }




    private FromToTSHGoogleSheets(bool empty) : this()
    {
        this.empty = empty;
    }







    public FromToTSHGoogleSheets(T from, T to, FromToUseGoogleSheets ftUse = FromToUseGoogleSheets.DateTime) : this()
    {
        this.from = from;
        this.to = to;
        this.ftUse = ftUse;
    }
    public T from
    {
        get => (T)(dynamic)fromL;
        set => fromL = (long)(dynamic)value;
    }
    public T to
    {
        get => (T)(dynamic)toL;
        set => toL = (long)(dynamic)value;
    }
    public long FromL => fromL;
    public long ToL => toL;
}