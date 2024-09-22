namespace Idasen.SystemTray.Win11.Interfaces
{
    public interface ISettingsWindow
    {
        void                                             Show ( ) ;
        void                                             Close ( ) ;

        event EventHandler                               AdvancedSettingsChanged ;
        event EventHandler<LockSettingsChangedEventArgs> LockSettingsChanged ;
    }
}