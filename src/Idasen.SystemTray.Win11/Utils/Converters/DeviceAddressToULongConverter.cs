using System.Globalization ;
using Idasen.SystemTray.Win11.Interfaces ;

namespace Idasen.SystemTray.Win11.Utils.Converters ;

public class DeviceAddressToULongConverter ( IStringToUIntConverter converter )
    : IDeviceAddressToULongConverter
{
    public ulong DefaultIfEmpty ( string deviceAddress )
    {
        return string.IsNullOrWhiteSpace ( deviceAddress )
                   ? AppConfiguration.Defaults.DeviceAddress
                   : converter.ConvertStringToUlongOrDefault ( deviceAddress ,
                                                               AppConfiguration.Defaults.DeviceAddress ) ;
    }

    public string EmptyIfDefault ( ulong deviceAddress )
    {
        return deviceAddress == AppConfiguration.Defaults.DeviceAddress
                   ? string.Empty
                   : deviceAddress.ToString ( CultureInfo.InvariantCulture ) ;
    }
}
