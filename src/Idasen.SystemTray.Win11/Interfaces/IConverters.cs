namespace Idasen.SystemTray.Win11.Interfaces ;

public interface IConverters
{
    IDoubleToUIntConverter         DoubleToUIntConverter { get ; }
    IDeviceNameConverter           DeviceNameConverter { get ; }
    IDeviceAddressToULongConverter DeviceAddressToULongConverter { get ; }
}
