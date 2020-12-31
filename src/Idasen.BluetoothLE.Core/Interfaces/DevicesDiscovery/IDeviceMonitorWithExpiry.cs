using System ;
using JetBrains.Annotations ;

namespace Idasen.BluetoothLE.Core.Interfaces.DevicesDiscovery
{
    public interface IDeviceMonitorWithExpiry
        : IDeviceMonitor
    {
        /// <summary>
        ///     Notification for expired devices.
        /// </summary>
        [ UsedImplicitly ]
        IObservable < IDevice > DeviceExpired { get ; }

        /// <summary>
        ///     Timespan after a device is expired and removed from the
        ///     collection of discovered devices.
        /// </summary>
        TimeSpan TimeOut { get ; set ; }
    }
}