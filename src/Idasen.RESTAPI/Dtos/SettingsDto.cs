namespace Idasen.RESTAPI.Dtos
{
    public class SettingsDto
    {
        private const string DefaultId                      = "Unknown" ;
        private const uint   DefaultHeightSeating           = 6000u ;
        private const uint   DefaultHeightStanding          = 12000u ;
        private const uint   DefaultDeviceMonitoringTimeout = 600; // in seconds
        private const bool   DefaultLocked                  = false;
        private const string DefaultDeviceName              = "Desk";
        private const ulong  DefaultDeviceAddress           = 250635178951455u;

        public        string Id                      { get ; set ; } = DefaultId ;
        public        uint   Seating                 { get ; set ; } = DefaultHeightSeating ;
        public        uint   Standing                { get ; set ; } = DefaultHeightStanding ;
        public        string DeviceName              { get;  set; }  = DefaultDeviceName;
        public        ulong  DeviceAddress           { get;  set; }  = DefaultDeviceAddress;
        public        uint   DeviceMonitoringTimeout { get;  set; }  = DefaultDeviceMonitoringTimeout;
        public        bool   DeviceLocked            { get;  set; }  = DefaultLocked;

        public override string ToString ( )
        {
            return $"[{nameof(Id)}: {Id}, "                                          +
                   $"{nameof(Seating)}: {Seating}, "                                 +
                   $"{nameof(Standing)}: {Standing}, "                               +
                   $"{nameof(DeviceName)}: {DeviceName}, "                           +
                   $"{nameof(DeviceAddress)}: {DeviceAddress}, "                     +
                   $"{nameof(DeviceMonitoringTimeout)}: {DeviceMonitoringTimeout}, " +
                   $"{nameof(DeviceLocked)}: {DeviceLocked}]";
        }
    }
}