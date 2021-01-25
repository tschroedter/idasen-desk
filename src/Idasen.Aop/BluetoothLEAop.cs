using System.Diagnostics.CodeAnalysis ;
using Autofac ;
using Idasen.Aop.Aspects ;
using Idasen.Aop.Interfaces ;
using Serilog ;

namespace Idasen.Aop
{
    // ReSharper disable once InconsistentNaming
    [ ExcludeFromCodeCoverage ]
    public class BluetoothLEAop
        : Module
    {
        protected override void Load ( ContainerBuilder builder )
        {
            builder.RegisterType < InvocationToTextConverter > ( )
                   .As < IInvocationToTextConverter > ( ) ;

            builder.Register ( c => new LogAspect ( c.Resolve < ILogger > ( ) ,
                                                    c.Resolve < IInvocationToTextConverter > ( ) ) ) ;
        }
    }
}