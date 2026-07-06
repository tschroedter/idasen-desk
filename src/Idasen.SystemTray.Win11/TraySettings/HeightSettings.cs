using Idasen.SystemTray.Win11.Utils ;

namespace Idasen.SystemTray.Win11.TraySettings ;

public class HeightSettings
{
    /// <summary>
    ///     The standing height of the desk in centimeters.
    /// </summary>
    public uint StandingHeightInCm { get ; set ; } = AppConfiguration.Defaults.HeightStandingInCm ;

    /// <summary>
    ///     The custom name for standing position (default: "Stand").
    /// </summary>
    public string StandingName { get ; set ; } = AppConfiguration.Hotkeys.StandingName ;

    /// <summary>
    ///     The seating height of the desk in centimeters.
    /// </summary>
    public uint SeatingHeightInCm { get ; set ; } = AppConfiguration.Defaults.HeightSeatingInCm ;

    /// <summary>
    ///     The custom name for seating position (default: "Sit").
    /// </summary>
    public string SeatingName { get ; set ; } = AppConfiguration.Hotkeys.SeatingName ;

    /// <summary>
    ///     The custom position 1 height of the desk in centimeters.
    /// </summary>
    public uint Custom1HeightInCm { get ; set ; } = AppConfiguration.Defaults.HeightStandingInCm ;

    /// <summary>
    ///     The custom position 2 height of the desk in centimeters.
    /// </summary>
    public uint Custom2HeightInCm { get ; set ; } = AppConfiguration.Defaults.HeightSeatingInCm ;

    /// <summary>
    ///     The custom name for position 1 (default: "Custom 1").
    /// </summary>
    public string Custom1Name { get ; set ; } = AppConfiguration.Hotkeys.Custom1Name ;

    /// <summary>
    ///     The custom name for position 2 (default: "Custom 2").
    /// </summary>
    public string Custom2Name { get ; set ; } = AppConfiguration.Hotkeys.Custom2Name ;

    /// <summary>
    ///     The minimum height the desk can move to.
    /// </summary>
    public uint DeskMinHeightInCm { get ; set ; } = AppConfiguration.Defaults.DeskMinHeightInCm ;

    /// <summary>
    ///     The maximum height the desk can move to.
    /// </summary>
    public uint DeskMaxHeightInCm { get ; set ; } = AppConfiguration.Defaults.DeskMaxHeightInCm ;

    /// <summary>
    ///     The last known height of the desk.
    /// </summary>
    public uint LastKnownDeskHeight { get ; set ; } = AppConfiguration.Defaults.DeskMinHeightInCm ;

    /// <summary>
    ///     Indicates whether the seating height is visible in the context menu.
    /// </summary>
    public bool StandingIsVisibleInContextMenu { get ; set ; } = true ;

    /// <summary>
    ///     Indicates whether the custom position 1 height is visible in the context menu.
    /// </summary>
    public bool SeatingIsVisibleInContextMenu { get ; set ; } = true ;

    /// <summary>
    ///     Indicates whether the custom position 2 height is visible in the context menu.
    /// </summary>
    public bool Custom1IsVisibleInContextMenu { get ; set ; } = true ;

    /// <summary>
    ///     Indicates whether the custom position 2 height is visible in the context menu.
    /// </summary>
    public bool Custom2IsVisibleInContextMenu { get ; set ; } = true ;
}
