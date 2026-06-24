namespace SunamoGoogleSheets._sunamo.SunamoExceptions;

internal sealed partial class Exceptions
{
    #region Other
    internal static string CheckBefore(string prefix)
    {
        return string.IsNullOrWhiteSpace(prefix) ? string.Empty : prefix + ": ";
    }

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

    internal static void TypeAndMethodName(string stackTraceLine, out string type, out string methodName)
    {
        var methodPath = stackTraceLine.Split("at ")[1].Trim();
        var fullMethodName = methodPath.Split("(")[0];
        var parts = fullMethodName.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries).ToList();
        methodName = parts[^1];
        parts.RemoveAt(parts.Count - 1);
        type = string.Join(".", parts);
    }

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
    internal static string? BadFormatOfElementInList(string prefix, object elementValue, string listName, Func<object, string> nullToStringOrDefaultFunction)
    {
        return CheckBefore(prefix) + " Bad format of element" + " " + nullToStringOrDefaultFunction(elementValue) +
        " in list " + listName;
    }

    internal static string? Custom(string prefix, string message)
    {
        return CheckBefore(prefix) + message;
    }
    #endregion

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
