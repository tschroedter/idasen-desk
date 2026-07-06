using Idasen.SystemTray.Win11.Interfaces ;

namespace Idasen.SystemTray.Win11.Utils ;

/// <summary>
///     Provides a collection of available keys for hotkey configuration.
/// </summary>
public class AvailableKeysProvider : IAvailableKeysProvider
{
    private static readonly IReadOnlyList < string > Keys = new List < string >
    {
        "Up" ,
        "Down" ,
        "Left" ,
        "Right" ,
        "1" ,
        "2" ,
        "3" ,
        "4" ,
        "5" ,
        "6" ,
        "7" ,
        "8" ,
        "9" ,
        "0" ,
        "F1" ,
        "F2" ,
        "F3" ,
        "F4" ,
        "F5" ,
        "F6" ,
        "F7" ,
        "F8" ,
        "F9" ,
        "F10" ,
        "F11" ,
        "F12"
    } ;

    /// <inheritdoc />
    public IReadOnlyList < string > AvailableKeys => Keys ;
}
