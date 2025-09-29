using System.Globalization ;
using Idasen.SystemTray.Win11.Interfaces ;

namespace Idasen.SystemTray.Win11.Utils.Converters ;

public class DeviceAddressToULongConverter ( IStringToUIntConverter converter )
    : IDeviceAddressToULongConverter
{
    public ulong DefaultIfEmpty ( string deviceAddress )
    {
        return string.IsNullOrWhiteSpace ( deviceAddress )
                   ? Constants.DefaultDeviceAddress
                   : converter.ConvertStringToUlongOrDefault ( deviceAddress ,
                                                               Constants.DefaultDeviceAddress ) ;
    }

    public string EmptyIfDefault ( ulong deviceAddress )
    {
        return deviceAddress == Constants.DefaultDeviceAddress
                   ? string.Empty
                   : deviceAddress.ToString ( CultureInfo.InvariantCulture ) ;
    }
}