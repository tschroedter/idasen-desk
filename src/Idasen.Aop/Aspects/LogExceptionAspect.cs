using System ;
using Castle.DynamicProxy ;
using Idasen.Aop.Interfaces ;
using JetBrains.Annotations ;
using Serilog ;

namespace Idasen.Aop.Aspects
{
    public class LogExceptionAspect : IInterceptor
    {
        // todo move ErrorManager to common space
        public LogExceptionAspect ( [ NotNull ] ILogger                    logger ,
                                    [ NotNull ] IInvocationToTextConverter converter )
        {
            _logger    = logger ;
            _converter = converter ;
        }

        public void Intercept ( IInvocation invocation )
        {
            try
            {
                invocation.Proceed ( ) ;
            }
            catch ( Exception exception )
            {
                _logger.Error ( $"{_converter.Convert ( invocation )} " +
                                exception ) ;
            }
        }

        private readonly IInvocationToTextConverter _converter ;
        private readonly ILogger                    _logger ;
    }
}