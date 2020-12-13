using System.Threading.Tasks ;

namespace Idasen.SystemTray.Interfaces
{
    public interface ISettingsManager
    {
        ISettings CurrentSettings { get ; }

        Task Save ( ) ;
        Task Load ( ) ;
    }
}