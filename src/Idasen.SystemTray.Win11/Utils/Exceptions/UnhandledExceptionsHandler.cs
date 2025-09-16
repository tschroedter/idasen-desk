using System.Diagnostics.CodeAnalysis ;
using System.Windows.Threading ;
using Idasen.BluetoothLE.Characteristics.Common ;
using Serilog ;

namespace Idasen.SystemTray.Win11.Utils.Exceptions ;

[ ExcludeFromCodeCoverage ]
public static class UnhandledExceptionsHandler
{
    private static readonly ErrorHandler ErrorHandler = new ( ) ;
    private static          bool         _registered ;

    public static void RegisterGlobalExceptionHandling ( )
    {
        if ( _registered )
            return ;

        // Reuse the application-wide configured Serilog logger to avoid duplicate sinks
        var logger = Log.Logger ;

        logger.Debug ( "Registering global exception handlers..." ) ;

        // AppDomain unhandled exceptions
        AppDomain.CurrentDomain.UnhandledException +=
            ( _ , args ) => CurrentDomainOnUnhandledException ( args ,
                                                                logger ) ;

        // Register a single dispatcher-level handler to avoid duplicate logging
        Application.Current.DispatcherUnhandledException +=
            ( _ , args ) => CurrentOnDispatcherUnhandledException ( args ,
                                                                    logger ) ;

        // Observe unhandled task exceptions
        TaskScheduler.UnobservedTaskException +=
            ( _ , args ) => TaskSchedulerOnUnobservedTaskException ( args ,
                                                                     logger ) ;

        _registered = true ;
    }

    private static void TaskSchedulerOnUnobservedTaskException (
        UnobservedTaskExceptionEventArgs args ,
        ILogger                          log )
    {
        ErrorHandler.Handle ( args.Exception ,
                              log ) ;

        args.SetObserved ( ) ;
    }

    private static void CurrentOnDispatcherUnhandledException (
        DispatcherUnhandledExceptionEventArgs args ,
        ILogger                               log )
    {
        if ( args.Exception.IsBluetoothDisabledException ( ) )
        {
            HandleBluetoothDisabledException ( args ,
                                               log ) ;
        }
        else
        {
            HandleGeneralException ( args ,
                                     log ) ;
        }
    }

    private static void HandleGeneralException ( DispatcherUnhandledExceptionEventArgs args ,
                                                 ILogger                               log )
    {
        log.Error ( args.Exception ,
                    args.Exception.Message ) ;
    }

    private static void HandleBluetoothDisabledException ( DispatcherUnhandledExceptionEventArgs args ,
                                                           ILogger                               log )
    {
        args.Exception.LogBluetoothStatusException ( log ,
                                                     string.Empty ) ;

        args.Handled = true ;
    }

    private static void CurrentDomainOnUnhandledException ( UnhandledExceptionEventArgs args ,
                                                            ILogger                     log )
    {
        var exception = args.ExceptionObject as Exception ;
        var terminatingMessage = args.IsTerminating
                                     ? " The application is terminating."
                                     : string.Empty ;
        var exceptionMessage = exception?.Message ?? "An unmanaged exception occurred." ;
        var message = string.Concat ( exceptionMessage ,
                                      terminatingMessage ) ;

        if ( exception != null &&
             exception.IsBluetoothDisabledException ( ) )
            exception.LogBluetoothStatusException ( log ,
                                                    string.Empty ) ;
        else
            log.Error ( exception ,
                        message ) ;
    }
}