using Idasen.BluetoothLE.Characteristics.Common ;
using Serilog ;

namespace Idasen.SystemTray.Win11.Utils.Exceptions ;

public class BluetoothDisabledExceptionHandler : IExceptionHandler
{
    public bool CanHandle(Exception exception)
    {
        return exception.IsBluetoothDisabledException();
    }

    public void Handle(Exception exception, ILogger logger)
    {
        exception.LogBluetoothStatusException(logger, string.Empty);
    }
}