using System.Diagnostics.CodeAnalysis ;
using Autofac ;
using Autofac.Extras.DynamicProxy ;
using Idasen.Aop ;
using Idasen.RESTAPI.Desks ;
using Idasen.RESTAPI.Interfaces ;

namespace Idasen.RESTAPI
{
    // ReSharper disable once InconsistentNaming
    [ ExcludeFromCodeCoverage ]
    public class IdasenRESTAPIModule
        : Module
    {
        protected override void Load ( ContainerBuilder builder )
        {
            builder.RegisterModule < BluetoothLEAop > ( ) ;

            builder.RegisterType < DeskManager > ( )
                   .As < IDeskManager > ( )
                   .EnableInterfaceInterceptors ( ) ;
        }
    }
}