namespace SunamoGoogleSheets._sunamo.SunamoExceptions;

/// <summary>
/// Provides exception message formatting utilities
/// </summary>
internal sealed partial class Exceptions
{
    #region Other
    /// <summary>
    /// Checks if the prefix text is empty and formats it with a colon if not
    /// </summary>
    /// <param name="prefix">The prefix text to check</param>
    /// <returns>Empty string if prefix is empty, otherwise prefix with colon and space</returns>
    internal static string CheckBefore(string prefix)
    {
        return string.IsNullOrWhiteSpace(prefix) ? string.Empty : prefix + ": ";
    }

    /// <summary>
    /// Gets the location where the exception occurred in the call stack
    /// </summary>
    /// <param name="isFilling AlsoFirstTwo">If true, fills the first two items (type and method name)</param>
    /// <returns>Tuple containing type name, method name, and full stack trace</returns>
    internal static Tuple<string, string, string> PlaceOfException(bool isFillingAlsoFirstTwo = true)
    {
        StackTrace stackTrace = new();
        var stackTraceText = stackTrace.ToString();
        var lines = stackTraceText.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
        lines.RemoveAt(0);
        var i = 0;
        string type = string.Empty;
        string methodName = string.Empty;
        for (; i < lines.Count; i++)
        {
            var item = lines[i];
            if (isFillingAlsoFirstTwo)
                if (!item.StartsWith("   at ThrowEx"))
                {
                    TypeAndMethodName(item, out type, out methodName);
                    isFillingAlsoFirstTwo = false;
                }
            if (item.StartsWith("at System."))
            {
                lines.Add(string.Empty);
                lines.Add(string.Empty);
                break;
            }
        }
        return new Tuple<string, string, string>(type, methodName, string.Join(Environment.NewLine, lines));
    }

    /// <summary>
    /// Extracts type and method name from a stack trace line
    /// </summary>
    /// <param name="stackTraceLine">The stack trace line to parse</param>
    /// <param name="type">Output: the type name</param>
    /// <param name="methodName">Output: the method name</param>
    internal static void TypeAndMethodName(string stackTraceLine, out string type, out string methodName)
    {
        var methodPath = stackTraceLine.Split("at ")[1].Trim();
        var fullMethodName = methodPath.Split("(")[0];
        var parts = fullMethodName.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries).ToList();
        methodName = parts[^1];
        parts.RemoveAt(parts.Count - 1);
        type = string.Join(".", parts);
    }

    /// <summary>
    /// Gets the name of the calling method
    /// </summary>
    /// <param name="frameDepth">The depth in the call stack (1 = immediate caller)</param>
    /// <returns>The name of the calling method</returns>
    internal static string CallingMethod(int frameDepth = 1)
    {
        StackTrace stackTrace = new();
        var methodBase = stackTrace.GetFrame(frameDepth)?.GetMethod();
        if (methodBase == null)
        {
            return "Method name cannot be get";
        }
        var methodName = methodBase.Name;
        return methodName;
    }
    #endregion

    #region IsNullOrWhitespace
    internal readonly static StringBuilder AdditionalInfoInnerStringBuilder = new();
    internal readonly static StringBuilder AdditionalInfoStringBuilder = new();
    #endregion

    #region OnlyReturnString
    /// <summary>
    /// Creates an error message for a bad format of an element in a list
    /// </summary>
    /// <param name="prefix">The prefix text for the message</param>
    /// <param name="elementValue">The element value that has bad format</param>
    /// <param name="listName">The name of the list containing the element</param>
    /// <param name="nullToStringOrDefaultFunction">Function to convert null values to string</param>
    /// <returns>Formatted error message</returns>
    internal static string? BadFormatOfElementInList(string prefix, object elementValue, string listName, Func<object, string> nullToStringOrDefaultFunction)
    {
        return CheckBefore(prefix) + " Bad format of element" + " " + nullToStringOrDefaultFunction(elementValue) +
        " in list " + listName;
    }

    /// <summary>
    /// Creates a custom error message with a prefix
    /// </summary>
    /// <param name="prefix">The prefix text for the message</param>
    /// <param name="message">The custom message</param>
    /// <returns>Formatted error message</returns>
    internal static string? Custom(string prefix, string message)
    {
        return CheckBefore(prefix) + message;
    }
    #endregion

    /// <summary>
    /// Creates an error message for a not implemented case
    /// </summary>
    /// <param name="prefix">The prefix text for the message</param>
    /// <param name="notImplementedName">The name or type of the not implemented case</param>
    /// <returns>Formatted error message</returns>
    internal static string? NotImplementedCase(string prefix, object notImplementedName)
    {
        var typeInfo = string.Empty;
        if (notImplementedName != null)
        {
            typeInfo = " for ";
            if (notImplementedName.GetType() == typeof(Type))
                typeInfo += ((Type)notImplementedName).FullName;
            else
                typeInfo += notImplementedName.ToString();
        }
        return CheckBefore(prefix) + "Not implemented case" + typeInfo + " . internal program error. Please contact developer" +
        ".";
    }
}