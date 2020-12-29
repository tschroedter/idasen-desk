namespace Idasen.SystemTray.Interfaces
{
    public interface ISettings
    {
        uint   StandingHeightInCm { get ; set ; }
        uint   SeatingHeightInCm  { get ; set ; }
        string DeviceName         { get;  set; }
        ulong  DeviceAddress      { get;  set; }
    }
}