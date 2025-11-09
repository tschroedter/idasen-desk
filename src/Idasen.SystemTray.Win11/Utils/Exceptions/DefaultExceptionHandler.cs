using Idasen.SystemTray.Win11.Interfaces ;
using Serilog ;

namespace Idasen.SystemTray.Win11.Utils.Exceptions ;

public class DefaultExceptionHandler : IExceptionHandler
{
    public bool CanHandle ( Exception exception )
    {
        return true ;
    }

    public void Handle ( Exception exception , ILogger logger )
    {
#pragma warning disable CA2254
        logger.Error ( exception ,
                       exception.Message ) ;
#pragma warning restore CA2254
    }
}