using Idasen.SystemTray.Interfaces ;

namespace Idasen.SystemTray.Utils
{
    public class DeviceAddressToULongConverter
        : IDeviceAddressToULongConverter
    {
        private readonly IStringToUIntConverter _stringConverter = new StringToUIntConverter();

        public ulong DefaultIfEmpty ( string deviceAddress )
        {
            return string.IsNullOrWhiteSpace ( deviceAddress )
                       ? Constants.DefaultDeviceAddress
                       : _stringConverter.ConvertToULong ( deviceAddress ,
                                                           Constants.DefaultDeviceAddress ) ;
        }

        public string EmptyIfDefault ( ulong deviceAddress )
        {
            return deviceAddress == Constants.DefaultDeviceAddress
                       ? string.Empty
                       : deviceAddress.ToString ( ) ;
        }
    }
}