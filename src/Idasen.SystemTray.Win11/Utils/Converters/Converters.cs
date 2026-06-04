using Idasen.SystemTray.Win11.Interfaces ;

namespace Idasen.SystemTray.Win11.Utils.Converters ;

public class Converters (
    IDoubleToUIntConverter         doubleToUIntConverter ,
    IDeviceNameConverter           deviceNameConverter ,
    IDeviceAddressToULongConverter deviceAddressToULongConverter )
    : IConverters
{
    public IDoubleToUIntConverter         DoubleToUIntConverter         { get ; } = doubleToUIntConverter ;
    public IDeviceNameConverter           DeviceNameConverter           { get ; } = deviceNameConverter ;
    public IDeviceAddressToULongConverter DeviceAddressToULongConverter { get ; } = deviceAddressToULongConverter ;
}
