namespace SunamoGoogleSheets._public.SunamoData.Data;

public class FromToTSHGoogleSheets<T>
{
    private long fromLongValue;
    private long toLongValue;

    public bool Empty { get; set; }

    public FromToUseGoogleSheets TimestampFormat { get; set; } = FromToUseGoogleSheets.DateTime;

    public FromToTSHGoogleSheets()
    {
        var type = typeof(T);
        if (type == typeof(int)) TimestampFormat = FromToUseGoogleSheets.None;
    }

    private FromToTSHGoogleSheets(bool isEmpty) : this()
    {
        this.Empty = isEmpty;
    }

    public FromToTSHGoogleSheets(T fromValue, T toValue, FromToUseGoogleSheets timestampFormat = FromToUseGoogleSheets.DateTime) : this()
    {
        this.From = fromValue;
        this.To = toValue;
        this.TimestampFormat = timestampFormat;
    }

    public T From
    {
        get => (T)(dynamic)fromLongValue!;
        set => fromLongValue = (long)(dynamic)value!;
    }

    public T To
    {
        get => (T)(dynamic)toLongValue!;
        set => toLongValue = (long)(dynamic)value!;
    }

    public long FromL => fromLongValue;

    public long ToL => toLongValue;
}
