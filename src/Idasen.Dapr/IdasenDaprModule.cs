using System.Diagnostics.CodeAnalysis ;
using Autofac ;
using Autofac.Extras.DynamicProxy ;
using Idasen.Aop ;

namespace Idasen.Dapr
{
    // ReSharper disable once InconsistentNaming
    [ ExcludeFromCodeCoverage ]
    public class IdasenDaprModule
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