using Idasen.SystemTray.Win11.Utils ;

namespace Idasen.SystemTray.Win11.TraySettings ;

public class HotkeySettings
{
    /// <summary>
    ///     Indicates whether global hotkeys are enabled.
    ///     Set to false to disable all global hotkeys.
    /// </summary>
    public bool GlobalHotkeysEnabled { get ; set ; } = Constants.DefaultGlobalHotkeysEnabled ;

    /// <summary>
    ///     The custom name for standing position (default: "Stand").
    /// </summary>
    public string StandingName { get ; set ; } = Constants.DefaultStandingName ;

    /// <summary>
    ///     The key for the Standing hotkey (e.g., "Up").
    /// </summary>
    public string StandingKey { get ; set ; } = Constants.DefaultStandingKey ;

    /// <summary>
    ///     The modifier keys for the Standing hotkey (e.g., "Control, Alt, Shift").
    /// </summary>
    public string StandingModifiers { get ; set ; } = Constants.DefaultHotkeyModifiers ;

    /// <summary>
    ///     The key for the Seating hotkey (e.g., "Down").
    /// </summary>
    public string SeatingKey { get ; set ; } = Constants.DefaultSeatingKey ;

    /// <summary>
    ///     The modifier keys for the Seating hotkey (e.g., "Control, Alt, Shift").
    /// </summary>
    public string SeatingModifiers { get ; set ; } = Constants.DefaultHotkeyModifiers ;

    /// <summary>
    ///     The key for the Custom1 hotkey (e.g., "Left").
    /// </summary>
    public string Custom1Key { get ; set ; } = Constants.DefaultCustom1Key ;

    /// <summary>
    ///     The modifier keys for the Custom1 hotkey (e.g., "Control, Alt, Shift").
    /// </summary>
    public string Custom1Modifiers { get ; set ; } = Constants.DefaultHotkeyModifiers ;

    /// <summary>
    ///     The key for the Custom2 hotkey (e.g., "Right").
    /// </summary>
    public string Custom2Key { get ; set ; } = Constants.DefaultCustom2Key ;

    /// <summary>
    ///     The modifier keys for the Custom2 hotkey (e.g., "Control, Alt, Shift").
    /// </summary>
    public string Custom2Modifiers { get ; set ; } = Constants.DefaultHotkeyModifiers ;
}
