namespace Idasen.SystemTray.Win11.Interfaces ;

/// <summary>
///     Provides a collection of available keys for hotkey configuration.
/// </summary>
public interface IAvailableKeysProvider
{
    /// <summary>
    ///     Gets the collection of available keys that can be used for hotkey configuration.
    /// </summary>
    IReadOnlyList < string > AvailableKeys { get ; }
}
