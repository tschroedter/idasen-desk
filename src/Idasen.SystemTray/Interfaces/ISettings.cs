namespace Idasen.SystemTray.Interfaces
{
    public interface ISettings
    {
        /// <summary>
        ///     The standing height of the desk in centimeters.
        /// </summary>
        uint StandingHeightInCm { get ; set ; }

        /// <summary>
        ///     The seating height of the desk in centimeters.
        /// </summary>
        uint SeatingHeightInCm { get ; set ; }

        /// <summary>
        ///     The device's name which is used to discover a desk.
        /// </summary>
        string DeviceName { get ; set ; }

        /// <summary>
        ///     The device's Bluetooth address which is used to discover a desk..
        /// </summary>
        ulong DeviceAddress { get ; set ; }

        /// <summary>
        ///     Device monitoring timeout in seconds which is used to empty a
        ///     cache of monitored devices.
        /// </summary>
        uint DeviceMonitoringTimeout { get ; set ; }

        bool DeviceLocked { get ; set ; }
    }
}