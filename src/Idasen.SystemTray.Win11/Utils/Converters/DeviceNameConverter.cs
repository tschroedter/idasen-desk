using Idasen.SystemTray.Win11.Interfaces ;

namespace Idasen.SystemTray.Win11.Utils.Converters ;

public class DeviceNameConverter
    : IDeviceNameConverter
{
    public string DefaultIfEmpty ( string deviceName )
    {
        return string.IsNullOrWhiteSpace ( deviceName )
                   ? AppConfiguration.Defaults.DeviceName
                   : deviceName ;
    }

    public string EmptyIfDefault ( string deviceName )
    {
        return deviceName == AppConfiguration.Defaults.DeviceName
                   ? string.Empty
                   : deviceName ;
    }
}
