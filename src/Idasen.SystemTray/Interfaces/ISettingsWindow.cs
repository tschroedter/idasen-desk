using System ;

namespace Idasen.SystemTray.Interfaces
{
    public interface ISettingsWindow
    {
        void               Show ( ) ;
        void               Close ( ) ;
        event EventHandler AdvancedSettingsChanged ;
    }
}