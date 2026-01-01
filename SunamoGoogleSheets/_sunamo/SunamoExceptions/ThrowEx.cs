namespace SunamoGoogleSheets._sunamo.SunamoExceptions;

/// <summary>
/// Provides utilities for throwing exceptions with detailed stack trace information
/// </summary>
internal partial class ThrowEx
{
    /// <summary>
    /// Throws an exception for a bad format of an element in a list
    /// </summary>
    /// <param name="elementValue">The element value that has bad format</param>
    /// <param name="listName">The name of the list containing the element</param>
    /// <param name="nullToStringOrDefaultFunction">Function to convert null values to string</param>
    /// <returns>True if exception was thrown or would be thrown</returns>
    internal static bool BadFormatOfElementInList(
        object elementValue,
        string listName,
        Func<object, string> nullToStringOrDefaultFunction)
    {
        return ThrowIsNotNull(
            Exceptions.BadFormatOfElementInList(FullNameOfExecutedCode(), elementValue, listName, nullToStringOrDefaultFunction));
    }

    /// <summary>
    /// Throws a custom exception with an optional second message
    /// </summary>
    /// <param name="message">The main exception message</param>
    /// <param name="isReallyThrowing">If true, actually throws the exception; if false, only returns true</param>
    /// <param name="secondMessage">Optional second message to append</param>
    /// <returns>True if exception was thrown or would be thrown</returns>
    internal static bool Custom(string message, bool isReallyThrowing = true, string secondMessage = "")
    {
        string joined = string.Join(" ", message, secondMessage);
        string? exceptionMessage = Exceptions.Custom(FullNameOfExecutedCode(), joined);
        return ThrowIsNotNull(exceptionMessage, isReallyThrowing);
    }

    /// <summary>
    /// Throws an exception for a not implemented case
    /// </summary>
    /// <param name="notImplementedName">The name or type of the not implemented case</param>
    /// <returns>True if exception was thrown or would be thrown</returns>
    internal static bool NotImplementedCase(object notImplementedName)
    { return ThrowIsNotNull(Exceptions.NotImplementedCase, notImplementedName); }

    #region Other
    /// <summary>
    /// Gets the full name of the currently executed code (type and method)
    /// </summary>
    /// <returns>Full name in format TypeName.MethodName</returns>
    internal static string FullNameOfExecutedCode()
    {
        Tuple<string, string, string> placeOfException = Exceptions.PlaceOfException();
        string executedCodeFullName = FullNameOfExecutedCode(placeOfException.Item1, placeOfException.Item2, true);
        return executedCodeFullName;
    }

    /// <summary>
    /// Gets the full name of the executed code from type and method name
    /// </summary>
    /// <param name="type">The type (can be Type, MethodBase, string, or any object)</param>
    /// <param name="methodName">The method name</param>
    /// <param name="isFromThrowEx">If true, adjusts the stack depth because called from ThrowEx</param>
    /// <returns>Full name in format TypeName.MethodName</returns>
    static string FullNameOfExecutedCode(object type, string methodName, bool isFromThrowEx = false)
    {
        if (methodName == null)
        {
            int depth = 2;
            if (isFromThrowEx)
            {
                depth++;
            }

            methodName = Exceptions.CallingMethod(depth);
        }
        string typeFullName;
        if (type is Type typeValue)
        {
            typeFullName = typeValue.FullName ?? "Type cannot be get via type is Type type2";
        }
        else if (type is MethodBase method)
        {
            typeFullName = method.ReflectedType?.FullName ?? "Type cannot be get via type is MethodBase method";
            methodName = method.Name;
        }
        else if (type is string)
        {
            typeFullName = type.ToString() ?? "Type cannot be get via type is string";
        }
        else
        {
            Type objectType = type.GetType();
            typeFullName = objectType.FullName ?? "Type cannot be get via type.GetType()";
        }
        return string.Concat(typeFullName, ".", methodName);
    }

    /// <summary>
    /// Throws an exception if the exception message is not null
    /// </summary>
    /// <param name="exception">The exception message</param>
    /// <param name="isReallyThrowing">If true, actually throws the exception; if false, only returns true</param>
    /// <returns>True if exception was thrown or would be thrown, false if exception message was null</returns>
    internal static bool ThrowIsNotNull(string? exception, bool isReallyThrowing = true)
    {
        if (exception != null)
        {
            Debugger.Break();
            if (isReallyThrowing)
            {
                throw new Exception(exception);
            }
            return true;
        }
        return false;
    }

    #region For avoid FullNameOfExecutedCode

    /// <summary>
    /// Throws an exception using a function that generates the exception message
    /// </summary>
    /// <typeparam name="A">The type of the argument to pass to the exception function</typeparam>
    /// <param name="exceptionFunction">The function that generates the exception message</param>
    /// <param name="argument">The argument to pass to the exception function</param>
    /// <returns>True if exception was thrown or would be thrown</returns>
    internal static bool ThrowIsNotNull<A>(Func<string, A, string?> exceptionFunction, A argument)
    {
        string? exceptionMessage = exceptionFunction(FullNameOfExecutedCode(), argument);
        return ThrowIsNotNull(exceptionMessage);
    }

    #endregion
    #endregion
}