namespace Idasen.SystemTray.Win11.Interfaces ;

public interface IDeviceNameConverter
{
    string DefaultIfEmpty ( string deviceName ) ;
    string EmptyIfDefault ( string deviceName ) ;
}