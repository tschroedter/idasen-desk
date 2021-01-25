using System ;
using Castle.DynamicProxy ;
using Idasen.Aop.Interfaces ;
using JetBrains.Annotations ;
using Serilog ;
using Serilog.Events ;

namespace Idasen.Aop.Aspects
{
    public class LogAspect : IInterceptor
    {
        public LogAspect ( [ NotNull ] ILogger                    logger ,
                           [ NotNull ] IInvocationToTextConverter converter )
        {
            _logger    = logger ;
            _converter = converter ;
        }

        public void Intercept ( IInvocation invocation )
        {
            if ( Log.IsEnabled ( LogEventLevel.Debug ) )
                _logger.Debug ( "[LogAspect] " +
                                _converter.Convert ( invocation ) ) ;

            try
            {
                invocation.Proceed ( ) ;
            }
            catch ( Exception exception )
            {
                if ( Log.IsEnabled ( LogEventLevel.Error ) )
                    _logger.Error ( _converter.Convert ( invocation ) ,
                                    exception ) ;

                throw ;
            }
        }

        private readonly IInvocationToTextConverter _converter ;
        private readonly ILogger                    _logger ;
    }
}