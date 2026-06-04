using Idasen.SystemTray.Win11.Utils ;

namespace Idasen.SystemTray.Win11.Interfaces ;

public interface IStatusBarManager
{
    IObservable < StatusBarInfo > StatusBarInfoChanged { get ; }

    void UpdateStatus ( StatusBarInfo info ) ;
    void UpdateDeskHeight ( uint heightInMillimeters ) ;
}
