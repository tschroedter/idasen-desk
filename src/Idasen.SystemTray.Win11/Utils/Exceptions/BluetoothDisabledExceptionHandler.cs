using Idasen.BluetoothLE.Characteristics.Common ;
using Idasen.SystemTray.Win11.Interfaces ;
using Serilog ;

namespace Idasen.SystemTray.Win11.Utils.Exceptions ;

public class BluetoothDisabledExceptionHandler : IExceptionHandler
{
    public bool CanHandle ( Exception exception )
    {
        return exception.IsBluetoothDisabledException ( ) ;
    }

    public void Handle ( Exception exception , ILogger logger )
    {
        // Log a friendly information message for the end user
        logger.Warning ( "Bluetooth seems to be disabled or unavailable. Please enable Bluetooth in Windows settings and try again." ) ;

        // Keep the specific status logging for diagnostics
        exception.LogBluetoothStatusException ( logger ,
                                                string.Empty ) ;
    }
}