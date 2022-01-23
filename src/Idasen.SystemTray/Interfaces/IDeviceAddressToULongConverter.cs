namespace Idasen.SystemTray.Interfaces
{
    public interface IDeviceAddressToULongConverter
    {
        ulong  DefaultIfEmpty ( string deviceAddress ) ;
        string EmptyIfDefault ( ulong deviceAddress ) ;
    }
}