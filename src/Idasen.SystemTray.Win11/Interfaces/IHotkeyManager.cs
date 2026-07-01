namespace Idasen.SystemTray.Win11.Interfaces ;

public interface IHotkeyManager : IDisposable
{
    void RegisterGlobalHotkeys ( ) ;
    void UnregisterGlobalHotkeys ( ) ;

    event EventHandler < EventArgs > ? StandingHotkeyPressed ;
    event EventHandler < EventArgs > ? SeatingHotkeyPressed ;
    event EventHandler < EventArgs > ? Custom1HotkeyPressed ;
    event EventHandler < EventArgs > ? Custom2HotkeyPressed ;
}
