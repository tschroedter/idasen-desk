using System ;
using System.Diagnostics.CodeAnalysis ;
using System.Threading.Tasks ;
using System.Windows ;
using System.Windows.Threading ;
using Idasen.BluetoothLE.Characteristics.Common ;
using Idasen.Launcher ;
using Serilog ;

namespace Idasen.SystemTray
{
    [ ExcludeFromCodeCoverage ]
    public static class UnhandledExceptionsHandler
    {

        public static void RegisterGlobalExceptionHandling ( )
        {
            var logger = LoggerProvider.CreateLogger ( Utils.Constants.ApplicationName ,
                                                       Utils.Constants.LogFilename ) ;

            logger.Debug ( "Registering global exception handlers..." ) ;

            // this is the line you really want
            AppDomain.CurrentDomain.UnhandledException +=
            ( sender ,
              args ) => CurrentDomainOnUnhandledException ( args ,
                                                            logger ) ;

            // optional: hooking up some more handlers, remember that you need
            // to hook up additional handlers when logging from other dispatchers,
            // schedulers, or applications

            Application.Current.Dispatcher.UnhandledException +=
            ( sender ,
              args ) => DispatcherOnUnhandledException ( args ,
                                                         logger ) ;

            Application.Current.DispatcherUnhandledException +=
            ( sender ,
              args ) => CurrentOnDispatcherUnhandledException ( args ,
                                                                logger ) ;

            TaskScheduler.UnobservedTaskException +=
            ( sender ,
              args ) => TaskSchedulerOnUnobservedTaskException ( args ,
                                                                 logger ) ;
        }

        private static void TaskSchedulerOnUnobservedTaskException (
            UnobservedTaskExceptionEventArgs args ,
            ILogger                          log )
        {
            if ( args.Exception.IsBluetoothDisabledException ( ) )
                args.Exception.LogBluetoothStatusException( log ) ;
            else
                log.Error ( args.Exception ,
                            args.Exception != null
                                ? args.Exception.Message
                                : "Message is null" ) ;

            args.SetObserved ( ) ;
        }

        private static void CurrentOnDispatcherUnhandledException (
            DispatcherUnhandledExceptionEventArgs args ,
            ILogger                               log )
        {
            if ( args.Exception.IsBluetoothDisabledException ( ) )
            {
                args.Exception.LogBluetoothStatusException(log);

                args.Handled = true ;
            }
            else
            {
                log.Error ( args.Exception ,
                            args.Exception.Message ) ;
            }
        }

        private static void DispatcherOnUnhandledException ( DispatcherUnhandledExceptionEventArgs args ,
                                                             ILogger                               log )
        {
            if ( args.Exception.IsBluetoothDisabledException ( ) )
            {
                args.Exception.LogBluetoothStatusException(log);

                args.Handled = true ;
            }
            else
            {
                log.Error ( args.Exception ,
                            args.Exception.Message ) ;
            }
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

            if ( exception.IsBluetoothDisabledException ( ) )
                exception.LogBluetoothStatusException(log);
            else
                log.Error ( exception ,
                            message ) ;
        }
    }
}