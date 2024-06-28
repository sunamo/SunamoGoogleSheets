namespace SunamoGoogleSheets;


// Musí být ve SunamoEnums protože je sdílen např. i ve SunamoDateTime
/// <summary>
/// For projects for which is reference whole Xlf useless
/// But it is only one file (like here Langs), consider import it instead create standalone project
/// Tool which will copy it automatilly also could not be bad
/// </summary>
internal enum Langs
{
    #region For easy copying to other files
    cs = 0,
    en = 1
    #endregion
}