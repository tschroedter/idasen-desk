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
    ///     The custom position 1 height of the desk in centimeters.
    /// </summary>
    public uint Custom1HeightInCm { get ; set ; } = Constants.DefaultHeightStandingInCm ;

    /// <summary>
    ///     The eating height of the desk in centimeters.
    /// </summary>
    public uint EatingHeightInCm { get ; set ; } = Constants.DefaultHeightSeatingInCm ;

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
    public uint LastKnownDeskHeight { get ; set ; } = Constants.DefaultDeskMinHeightInCm;
}