using System.Diagnostics.CodeAnalysis ;
using System.Windows.Threading ;
using Idasen.BluetoothLE.Characteristics.Common ;
using Idasen.Launcher ;
using Serilog ;

namespace Idasen.SystemTray.Win11.Utils.Exceptions ;

[ ExcludeFromCodeCoverage ]
public static class UnhandledExceptionsHandler
{
    private static readonly ErrorHandler ErrorHandler = new ( ) ;

    public static void RegisterGlobalExceptionHandling ( )
    {
        var logger = LoggerProvider.CreateLogger ( Constants.ApplicationName ,
                                                   Constants.LogFilename ) ;

        logger.Debug ( "Registering global exception handlers..." ) ;

        // this is the line you really want
        AppDomain.CurrentDomain.UnhandledException +=
        ( _ ,
          args ) => CurrentDomainOnUnhandledException ( args ,
                                                        logger ) ;

        // optional: hooking up some more handlers, remember that you need
        // to hook up additional handlers when logging from other dispatchers,
        // schedulers, or applications

        Application.Current.Dispatcher.UnhandledException +=
            ( _ , args ) => DispatcherOnUnhandledException ( args ,
                                                             logger ) ;

        Application.Current.DispatcherUnhandledException +=
            ( _ , args ) => CurrentOnDispatcherUnhandledException ( args ,
                                                                    logger ) ;

        TaskScheduler.UnobservedTaskException +=
            ( _ , args ) => TaskSchedulerOnUnobservedTaskException ( args ,
                                                                     logger ) ;
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

    private static void DispatcherOnUnhandledException ( DispatcherUnhandledExceptionEventArgs args ,
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