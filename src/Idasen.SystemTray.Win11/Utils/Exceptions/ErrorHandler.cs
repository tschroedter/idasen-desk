using Idasen.SystemTray.Win11.Interfaces ;
using Serilog ;

namespace Idasen.SystemTray.Win11.Utils.Exceptions ;

public class ErrorHandler
{
    private readonly List < IExceptionHandler > _handlers =
    [
        new BluetoothDisabledExceptionHandler ( ) ,
        new DefaultExceptionHandler ( )
    ] ;

    // should be last as it handles any exception
    public void Handle ( Exception exception , ILogger logger )
    {
        var handler = _handlers.FirstOrDefault( handler => handler.CanHandle ( exception ) ) ;

        handler?.Handle ( exception , logger ) ;
    }
}