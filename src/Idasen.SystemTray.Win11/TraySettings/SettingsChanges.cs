using System.Reactive.Subjects ;
using Idasen.SystemTray.Win11.Interfaces ;

namespace Idasen.SystemTray.Win11.TraySettings ;

public class SettingsChanges : IObserveSettingsChanges , INotifySettingsChanges
{
    public ISubject < ISettings >      StopChanged             { get ; } = new Subject < ISettings > ( ) ;
    public ISubject < HeightSettings > HeightSettingsChanged   { get ; } = new Subject < HeightSettings > ( ) ;
    public ISubject < bool >           LockSettingsChanged     { get ; } = new Subject < bool > ( ) ;
    public ISubject < bool >           AdvancedSettingsChanged { get ; } = new Subject < bool > ( ) ;

    IObservable < bool > IObserveSettingsChanges.          AdvancedSettingsChanged => AdvancedSettingsChanged ;
    IObservable < bool > IObserveSettingsChanges.          LockSettingsChanged     => LockSettingsChanged ;
    IObservable < HeightSettings > IObserveSettingsChanges.HeightSettingsChanged   => HeightSettingsChanged ;
    IObservable < ISettings > IObserveSettingsChanges.     StopChanged             => StopChanged ;
}