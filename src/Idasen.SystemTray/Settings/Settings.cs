using Idasen.SystemTray.Interfaces ;
using Idasen.SystemTray.Utils ;

namespace Idasen.SystemTray.Settings
{
    public class Settings : ISettings
    {
        public uint   StandingHeightInCm      { get ; set ; } = Constants.DefaultHeightStandingInCm ;
        public uint   SeatingHeightInCm       { get ; set ; } = Constants.DefaultHeightSeatingInCm ;
        public string DeviceName              { get ; set ; } = Constants.DefaultDeviceName ;
        public ulong  DeviceAddress           { get ; set ; } = Constants.DefaultDeviceAddress ;
        public uint   DeviceMonitoringTimeout { get ; set ; } = Constants.DefaultDeviceMonitoringTimeout ;
        public bool   DeviceLocked                  { get ; set ; } = Constants.DefaultLocked ;

        public override string ToString ( )
        {
            return $"{nameof ( StandingHeightInCm )} = {StandingHeightInCm}, "           +
                   $"{nameof ( SeatingHeightInCm )} = {SeatingHeightInCm}, "             +
                   $"{nameof ( DeviceName )} = {DeviceName}, "                           +
                   $"{nameof ( DeviceAddress )} = {DeviceAddress}, "                     +
                   $"{nameof ( DeviceMonitoringTimeout )} = {DeviceMonitoringTimeout}, " +
                   $"{nameof ( DeviceLocked )} = {DeviceLocked}" ;
        }
    }
}