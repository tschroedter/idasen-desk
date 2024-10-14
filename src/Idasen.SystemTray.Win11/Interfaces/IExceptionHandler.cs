using Serilog ;

namespace Idasen.SystemTray.Win11.Interfaces ;

internal interface IExceptionHandler
{
    bool CanHandle ( Exception exception ) ;

    void Handle ( Exception exception , ILogger logger ) ;
}