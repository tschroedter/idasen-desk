using Idasen.SystemTray.Win11.Utils ;

namespace Idasen.SystemTray.Win11.Interfaces ;

public interface IToastService
{
    void Show ( NotificationParameters parameters ) ;
}