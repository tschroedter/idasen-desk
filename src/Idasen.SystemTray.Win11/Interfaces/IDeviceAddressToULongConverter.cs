namespace Idasen.SystemTray.Win11.Interfaces ;

public interface IDeviceAddressToULongConverter
{
    ulong  DefaultIfEmpty ( string deviceAddress ) ;
    string EmptyIfDefault ( ulong  deviceAddress ) ;
}