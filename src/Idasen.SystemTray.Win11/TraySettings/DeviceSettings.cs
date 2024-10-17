using Idasen.SystemTray.Win11.Utils ;

namespace Idasen.SystemTray.Win11.TraySettings ;

public class DeviceSettings
{
    /// <summary>
    ///     The device's name which is used to discover a desk.
    /// </summary>
    public string DeviceName { get ; set ; } = Constants.DefaultDeviceName ;

    /// <summary>
    ///     The device's Bluetooth address which is used to discover a desk.
    /// </summary>
    public ulong DeviceAddress { get ; set ; } = Constants.DefaultDeviceAddress ;

    /// <summary>
    ///     Device monitoring timeout in seconds which is used to empty a
    ///     cache of monitored devices.
    /// </summary>
    public uint DeviceMonitoringTimeout { get ; set ; } = Constants.DefaultDeviceMonitoringTimeout ;

    /// <summary>
    ///     Locks the device to prevent any changes using the psychical controller on the desk.
    /// </summary>
    public bool DeviceLocked { get ; set ; } = Constants.DefaultLocked ;

    /// <summary>
    ///     Indicates if we should show a notifications.
    /// </summary>
    public bool NotificationsEnabled { get ; set ; } = Constants.NotificationsEnabled ;
}