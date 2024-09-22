﻿using Idasen.SystemTray.Win11.Interfaces ;

namespace Idasen.SystemTray.Win11.Utils
{
    public class DeviceNameConverter
        : IDeviceNameConverter
    {
        public string DefaultIfEmpty(string deviceName)
        {
            return string.IsNullOrWhiteSpace(deviceName)
                       ? Constants.DefaultDeviceName
                       : deviceName;
        }

        public string EmptyIfDefault(string deviceName)
        {
            return deviceName == Constants.DefaultDeviceName
                       ? string.Empty
                       : deviceName;
        }
    }
}