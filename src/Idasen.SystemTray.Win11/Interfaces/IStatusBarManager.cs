using Idasen.SystemTray.Win11.Utils ;
using JetBrains.Annotations;

namespace Idasen.SystemTray.Win11.Interfaces ;

public interface IStatusBarManager
{
    IObservable < StatusBarInfo > StatusBarInfoChanged { get ; }

    void UpdateStatus ( StatusBarInfo info ) ;

    [UsedImplicitly]
    void UpdateDeskHeight ( uint      heightInMillimeters ) ;
}
