using System.Reactive.Subjects ;
using Idasen.SystemTray.Win11.TraySettings ;

namespace Idasen.SystemTray.Win11.Interfaces ;

public interface INotifySettingsChanges
{
    ISubject < bool >           AdvancedSettingsChanged { get ; }
    ISubject < bool >           LockSettingsChanged     { get ; }
    ISubject < HeightSettings > HeightSettingsChanged   { get ; }
}