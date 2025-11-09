using Idasen.SystemTray.Win11.Interfaces ;
using Serilog ;

namespace Idasen.SystemTray.Win11.Utils.Exceptions ;

public class ErrorHandler
{
    internal readonly List < IExceptionHandler > Handlers =
    [
        new BluetoothDisabledExceptionHandler ( ) ,
        new DefaultExceptionHandler ( ) // this should be last and handle any exception
    ] ;

    // should be last as it handles any exception
    public void Handle ( Exception exception , ILogger logger )
    {
        var handler = Handlers.FirstOrDefault ( handler => handler.CanHandle ( exception ) ) ;

            handler?.Handle ( exception ,
                          logger ) ;

            if ( handler is null )
                throw new InvalidOperationException( $"No handler found for exception of type {exception.GetType().FullName}: {exception.Message}", 
                                                     exception);
    }
}