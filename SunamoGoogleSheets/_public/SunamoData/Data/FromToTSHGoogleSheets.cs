namespace SunamoGoogleSheets._public.SunamoData.Data;

/// <summary>
/// Base class for representing a range (from-to) of values with type conversion support
/// </summary>
/// <typeparam name="T">The type of values in the range</typeparam>
public class FromToTSHGoogleSheets<T>
{
    private long fromL;
    private long toL;

    /// <summary>
    /// Gets or sets a value indicating whether this range is empty
    /// </summary>
    public bool Empty { get; set; }

    /// <summary>
    /// Gets or sets the format to use when converting to/from timestamps
    /// </summary>
    public FromToUseGoogleSheets FtUse { get; set; } = FromToUseGoogleSheets.DateTime;

    /// <summary>
    /// Initializes a new instance of the FromToTSHGoogleSheets class
    /// </summary>
    public FromToTSHGoogleSheets()
    {
        var type = typeof(T);
        if (type == typeof(int)) FtUse = FromToUseGoogleSheets.None;
    }

    /// <summary>
    /// Initializes a new empty instance
    /// </summary>
    /// <param name="isEmpty">Whether this instance is empty</param>
    private FromToTSHGoogleSheets(bool isEmpty) : this()
    {
        this.Empty = isEmpty;
    }

    /// <summary>
    /// Initializes a new instance with from and to values
    /// </summary>
    /// <param name="fromValue">The from value</param>
    /// <param name="toValue">The to value</param>
    /// <param name="ftUse">The format to use when converting timestamps</param>
    public FromToTSHGoogleSheets(T fromValue, T toValue, FromToUseGoogleSheets ftUse = FromToUseGoogleSheets.DateTime) : this()
    {
        this.From = fromValue;
        this.To = toValue;
        this.FtUse = ftUse;
    }

    /// <summary>
    /// Gets or sets the from value
    /// </summary>
    public T From
    {
        get => (T)(dynamic)fromL;
        set => fromL = (long)(dynamic)value;
    }

    /// <summary>
    /// Gets or sets the to value
    /// </summary>
    public T To
    {
        get => (T)(dynamic)toL;
        set => toL = (long)(dynamic)value;
    }

    /// <summary>
    /// Gets the from value as a long
    /// </summary>
    public long FromL => fromL;

    /// <summary>
    /// Gets the to value as a long
    /// </summary>
    public long ToL => toL;
}