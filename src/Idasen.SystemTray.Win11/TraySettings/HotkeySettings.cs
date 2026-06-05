using Idasen.SystemTray.Win11.Utils ;

namespace Idasen.SystemTray.Win11.TraySettings ;

public class HotkeySettings
{
    /// <summary>
    ///     Indicates whether global hotkeys are enabled.
    ///     Set too false to disable all global hotkeys.
    /// </summary>
    public bool GlobalHotkeysEnabled { get ; set ; } = AppConfiguration.Defaults.GlobalHotkeysEnabled ;

    /// <summary>
    ///     The key for the Standing hotkey (e.g., "Up").
    /// </summary>
    public string StandingKey { get ; set ; } = AppConfiguration.Hotkeys.StandingKey ;

    /// <summary>
    ///     The modifier keys for the Standing hotkey (e.g., "Control, Alt, Shift").
    /// </summary>
    public string StandingModifiers { get ; set ; } = AppConfiguration.Hotkeys.DefaultModifiers ;

    /// <summary>
    ///     The key for the Seating hotkey (e.g., "Down").
    /// </summary>
    public string SeatingKey { get ; set ; } = AppConfiguration.Hotkeys.SeatingKey ;

    /// <summary>
    ///     The modifier keys for the Seating hotkey (e.g., "Control, Alt, Shift").
    /// </summary>
    public string SeatingModifiers { get ; set ; } = AppConfiguration.Hotkeys.DefaultModifiers ;

    /// <summary>
    ///     The key for the Custom1 hotkey (e.g., "Left").
    /// </summary>
    public string Custom1Key { get ; set ; } = AppConfiguration.Hotkeys.Custom1Key ;

    /// <summary>
    ///     The modifier keys for the Custom1 hotkey (e.g., "Control, Alt, Shift").
    /// </summary>
    public string Custom1Modifiers { get ; set ; } = AppConfiguration.Hotkeys.DefaultModifiers ;

    /// <summary>
    ///     The key for the Custom2 hotkey (e.g., "Right").
    /// </summary>
    public string Custom2Key { get ; set ; } = AppConfiguration.Hotkeys.Custom2Key ;

    /// <summary>
    ///     The modifier keys for the Custom2 hotkey (e.g., "Control, Alt, Shift").
    /// </summary>
    public string Custom2Modifiers { get ; set ; } = AppConfiguration.Hotkeys.DefaultModifiers ;
}
