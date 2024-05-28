using System ;
using Idasen.BluetoothLE.Characteristics.Common ;
using Serilog ;

namespace Idasen.SystemTray ;

public class BluetoothDisabledExceptionHandler : IExceptionHandler
{
    public bool CanHandle(Exception exception)
    {
        return exception.IsBluetoothDisabledException();
    }
    
    public void Handle(Exception exception, ILogger logger)
    {
        exception.LogBluetoothStatusException(logger);
    }
}