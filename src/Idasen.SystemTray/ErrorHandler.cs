using System ;
using System.Collections.Generic ;
using Serilog ;

namespace Idasen.SystemTray ;

public class ErrorHandler
{
    private readonly List<IExceptionHandler> _handlers = new List<IExceptionHandler>
                                                         {
                                                             new BluetoothDisabledExceptionHandler(),
                                                             new DefaultExceptionHandler() // should be last as it handles any exception
                                                         } ;

    // should be last as it handles any exception
    public void Handle(Exception exception, ILogger logger)
    {
        foreach (var handler in _handlers)
        {
            if (handler.CanHandle(exception))
            {
                handler.Handle(exception, logger);
                
                return;
            }
        }
    }
}