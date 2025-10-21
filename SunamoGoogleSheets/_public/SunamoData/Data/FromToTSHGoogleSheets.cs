// EN: Variable names have been checked and replaced with self-descriptive names
// CZ: Názvy proměnných byly zkontrolovány a nahrazeny samopopisnými názvy

namespace SunamoGoogleSheets._public.SunamoData.Data;

public class FromToTSHGoogleSheets<T>
{
    public bool empty;
    protected long fromL;
    public FromToUseGoogleSheets ftUse = FromToUseGoogleSheets.DateTime;
    protected long toL;

    public FromToTSHGoogleSheets()
    {
        var type = typeof(type);
        if (type == typeof(int)) ftUse = FromToUseGoogleSheets.None;
    }


    private FromToTSHGoogleSheets(bool empty) : this()
    {
        this.empty = empty;
    }


    public FromToTSHGoogleSheets(type from, type to, FromToUseGoogleSheets ftUse = FromToUseGoogleSheets.DateTime) : this()
    {
        this.from = from;
        this.to = to;
        this.ftUse = ftUse;
    }

    public type from
    {
        get => (type)(dynamic)fromL;
        set => fromL = (long)(dynamic)value;
    }

    public type to
    {
        get => (type)(dynamic)toL;
        set => toL = (long)(dynamic)value;
    }

    public long FromL => fromL;
    public long ToL => toL;
}