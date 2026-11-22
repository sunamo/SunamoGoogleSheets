namespace SunamoGoogleSheets._public.SunamoData.Data;

/// <summary>
/// Represents the from to TSH google sheets t type.
/// </summary>
public class FromToTSHGoogleSheets<T>
{
    private long fromLongValue;
    private long toLongValue;

    /// <summary>
    /// Gets or sets the empty.
    /// </summary>
    public bool Empty { get; set; }

    /// <summary>
    /// Gets or sets the timestamp format.
    /// </summary>
    public FromToUseGoogleSheets TimestampFormat { get; set; } = FromToUseGoogleSheets.DateTime;

    /// <summary>
    /// From to TSH google sheets.
    /// </summary>
    /// <returns>The from to TSH google sheets result.</returns>
    public FromToTSHGoogleSheets()
    {
        var type = typeof(T);
        if (type == typeof(int)) TimestampFormat = FromToUseGoogleSheets.None;
    }

    private FromToTSHGoogleSheets(bool isEmpty) : this()
    {
        this.Empty = isEmpty;
    }

    /// <summary>
    /// From to TSH google sheets.
    /// </summary>
    /// <param name="isEmpty">The is empty.</param>
    /// <returns>The from to TSH google sheets result.</returns>
    public FromToTSHGoogleSheets(T fromValue, T toValue, FromToUseGoogleSheets timestampFormat = FromToUseGoogleSheets.DateTime) : this()
    {
        this.From = fromValue;
        this.To = toValue;
        this.TimestampFormat = timestampFormat;
    }

    /// <summary>
    /// The from.
    /// </summary>
    public T From
    {
        get => (T)(dynamic)fromLongValue!;
        set => fromLongValue = (long)(dynamic)value!;
    }

    /// <summary>
    /// The to.
    /// </summary>
    public T To
    {
        get => (T)(dynamic)toLongValue!;
        set => toLongValue = (long)(dynamic)value!;
    }

    /// <summary>
    /// Gets the from l.
    /// </summary>
    public long FromL => fromLongValue;

    /// <summary>
    /// Gets the to l.
    /// </summary>
    public long ToL => toLongValue;
}
