using System.Collections.Generic ;
using Autofac ;
using Autofac.Core ;
using AutofacSerilogIntegration ;
using Idasen.BluetoothLE.Core ;
using Idasen.BluetoothLE.Linak ;
using Serilog ;

namespace Idasen.Launcher
{
    public static class ContainerProvider
    {
        public static IContainer Create (
            string                  appName ,
            string                  appLogFileName ,
            IEnumerable < IModule > otherModules = null )
        {
            Log.Logger = LoggerProvider.CreateLogger ( appName ,
                                                       appLogFileName ) ;

            var builder = new ContainerBuilder ( ) ;

            builder.RegisterLogger ( ) ;
            builder.RegisterModule < BluetoothLECoreModule > ( ) ;
            builder.RegisterModule < BluetoothLELinakModule > ( ) ;

            if ( otherModules == null )
                return builder.Build ( ) ;

            foreach ( var otherModule in otherModules )
            {
                builder.RegisterModule ( otherModule ) ;
            }

            return builder.Build ( ) ;
        }
    }
}