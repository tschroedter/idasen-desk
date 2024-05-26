using Idasen.SystemTray.Utils ;

namespace Idasen.SystemTray.Interfaces;

public class DeviceSettings
{
    /// <summary>
    ///     The device's name which is used to discover a desk.
    /// </summary>
    public string DeviceName              { get ; set ; } = Constants.DefaultDeviceName ;

    /// <summary>
    ///     The device's Bluetooth address which is used to discover a desk.
    /// </summary>
    public ulong  DeviceAddress           { get ; set ; } = Constants.DefaultDeviceAddress ;

    /// <summary>
    ///     Device monitoring timeout in seconds which is used to empty a
    ///     cache of monitored devices.
    /// </summary>
    public uint   DeviceMonitoringTimeout { get ; set ; } = Constants.DefaultDeviceMonitoringTimeout ;

    public bool   DeviceLocked            { get ; set ; } = Constants.DefaultLocked ;

    public bool   NotificationsEnabled    { get ; set ; } = Constants.NotificationsEnabled;
}
