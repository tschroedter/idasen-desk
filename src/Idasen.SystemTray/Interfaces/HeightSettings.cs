using Idasen.SystemTray.Utils ;

namespace Idasen.SystemTray.Interfaces;

public class HeightSettings
{
    /// <summary>
    ///     The standing height of the desk in centimeters.
    /// </summary>
    public uint StandingHeightInCm { get ; set ; } = Constants.DefaultHeightStandingInCm ;

    /// <summary>
    ///     The seating height of the desk in centimeters.
    /// </summary>
    public uint SeatingHeightInCm  { get ; set ; } = Constants.DefaultHeightSeatingInCm ;

    /// <summary>
    ///     The minimum height the desk can move to.
    /// </summary>
    public uint DeskMinHeightInCm  { get ; set ; } = Constants.DefaultDeskMinHeightInCm ;

    /// <summary>
    ///     The maximum height the desk can move to.
    /// </summary>
    public uint DeskMaxHeightInCm  { get ; set ; } = Constants.DefaultDeskMaxHeightInCm ;
}
