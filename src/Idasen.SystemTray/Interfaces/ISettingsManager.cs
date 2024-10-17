using System.Threading.Tasks ;

namespace Idasen.SystemTray.Interfaces
{
    public interface ISettingsManager
    {
        ISettings     CurrentSettings  { get ; }
        string        SettingsFileName { get ; }
        Task          Save ( ) ;
        Task          Load ( ) ;
        Task < bool > UpgradeSettings ( ) ;
    }
}