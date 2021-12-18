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
        static ContainerProvider ( )
        {
            Builder = new ContainerBuilder ( ) ;
        }

        public static ContainerBuilder Builder { get ; private set ; }

        public static IContainer Create (
            string                  appName ,
            string                  appLogFileName ,
            IEnumerable < IModule > otherModules = null )
        {
            Log.Logger = LoggerProvider.CreateLogger ( appName ,
                                                       appLogFileName ) ;

            Builder.RegisterLogger ( ) ;
            Builder.RegisterModule < BluetoothLECoreModule > ( ) ;
            Builder.RegisterModule < BluetoothLELinakModule > ( ) ;

            if ( otherModules == null )
                return Builder.Build ( ) ;

            foreach ( var otherModule in otherModules )
            {
                Builder.RegisterModule ( otherModule ) ;
            }

            return Builder.Build ( ) ;
        }
    }
}