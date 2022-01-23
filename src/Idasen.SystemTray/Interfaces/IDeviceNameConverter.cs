namespace Idasen.SystemTray.Interfaces
{
    public interface IDeviceNameConverter
    {
        string DefaultIfEmpty ( string deviceName ) ;
        string EmptyIfDefault ( string deviceName ) ;
    }
}