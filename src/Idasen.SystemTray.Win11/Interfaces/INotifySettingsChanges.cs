using System.Reactive.Subjects ;

namespace Idasen.SystemTray.Win11.Interfaces ;

public interface INotifySettingsChanges
{
    void NotifyAdvancedSettingsChanged ( bool hasChanged ) ;
    void NotifyLockSettingsChanged     ( bool isLocked ) ;
}