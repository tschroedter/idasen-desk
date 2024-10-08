﻿using Idasen.SystemTray.Win11.Interfaces ;

namespace Idasen.SystemTray.Win11.Utils
{
    public class DeviceAddressToULongConverter
        : IDeviceAddressToULongConverter
    {
        private readonly IStringToUIntConverter _stringConverter = new StringToUIntConverter();

        public ulong DefaultIfEmpty ( string deviceAddress )
        {
            return string.IsNullOrWhiteSpace ( deviceAddress )
                       ? Constants.DefaultDeviceAddress
                       : _stringConverter.ConvertStringToUlongOrDefault ( deviceAddress ,
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