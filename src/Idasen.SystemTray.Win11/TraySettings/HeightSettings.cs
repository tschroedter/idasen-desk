using Idasen.SystemTray.Win11.Utils ;

namespace Idasen.SystemTray.Win11.TraySettings ;

public class HeightSettings
{
    /// <summary>
    ///     The standing height of the desk in centimeters.
    /// </summary>
    public uint StandingHeightInCm { get ; set ; } = Constants.DefaultHeightStandingInCm ;

    /// <summary>
    ///     The seating height of the desk in centimeters.
    /// </summary>
    public uint SeatingHeightInCm { get ; set ; } = Constants.DefaultHeightSeatingInCm ;

    /// <summary>
    ///     The custom name for seating position (default: "Sit").
    /// </summary>
    public string SeatingName { get ; set ; } = Constants.DefaultSeatingName ;

    /// <summary>
    ///     The custom position 1 height of the desk in centimeters.
    /// </summary>
    public uint Custom1HeightInCm { get ; set ; } = Constants.DefaultHeightStandingInCm ;

    /// <summary>
    ///     The custom position 2 height of the desk in centimeters.
    /// </summary>
    public uint Custom2HeightInCm { get ; set ; } = Constants.DefaultHeightSeatingInCm ;

    /// <summary>
    ///     The custom name for position 1 (default: "Custom 1").
    /// </summary>
    public string Custom1Name { get ; set ; } = Constants.DefaultCustom1Name ;

    /// <summary>
    ///     The custom name for position 2 (default: "Custom 2").
    /// </summary>
    public string Custom2Name { get ; set ; } = Constants.DefaultCustom2Name ;

    /// <summary>
    ///     The minimum height the desk can move to.
    /// </summary>
    public uint DeskMinHeightInCm { get ; set ; } = Constants.DefaultDeskMinHeightInCm ;

    /// <summary>
    ///     The maximum height the desk can move to.
    /// </summary>
    public uint DeskMaxHeightInCm { get ; set ; } = Constants.DefaultDeskMaxHeightInCm ;

    /// <summary>
    ///     The last known height of the desk.
    /// </summary>
    public uint LastKnownDeskHeight { get ; set ; } = Constants.DefaultDeskMinHeightInCm ;

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