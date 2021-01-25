using System ;
using System.Linq ;
using System.Text ;
using System.Text.Json ;
using Castle.DynamicProxy ;
using Idasen.Aop.Interfaces ;
using JetBrains.Annotations ;
using Serilog ;

namespace Idasen.Aop
{
    public class InvocationToTextConverter : IInvocationToTextConverter
    {
        public InvocationToTextConverter ( [ NotNull ] ILogger logger )
        {
            _logger = logger ?? throw new ArgumentNullException ( nameof ( logger ) ) ;
        }

        public string Convert ( IInvocation invocation )
        {
            var arguments = ConvertArgumentsToString ( invocation.Arguments ) ;

            var called = $"{invocation.TargetType.Name}.{invocation.Method.Name}({arguments})" ;

            return called ;
        }

        [ UsedImplicitly ]
        internal string ConvertArgumentsToString ( [ NotNull ] object [ ] arguments )
        {
            var builder = new StringBuilder ( 100 ) ;

            foreach ( var argument in arguments )
            {
                var argumentDescription = argument == null
                                              ? "null"
                                              : DumpObject ( argument ) ;

                builder.Append ( argumentDescription ).Append ( "," ) ;
            }

            if ( arguments.Any ( ) ) builder.Length -- ;

            return builder.ToString ( ) ;
        }

        private string DumpObject ( object argument )
        {
            try
            {
                var json = JsonSerializer.Serialize ( argument ) ;

                return json ;
            }
            catch ( Exception e )
            {
                _logger.Error ( e ,
                                "Failed to convert object " +
                                $"'{argument.GetType ( ).FullName}' to json" ) ;

                return string.Empty ;
            }
        }

        private readonly ILogger _logger ;
    }
}