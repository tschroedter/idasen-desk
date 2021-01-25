using System ;
using System.Diagnostics.CodeAnalysis ;
using Windows.Storage.Streams ;
using Autofac.Extras.DynamicProxy ;
using Idasen.Aop.Aspects ;
using Idasen.BluetoothLE.Characteristics.Interfaces.Common ;
using Idasen.BluetoothLE.Core ;
using Serilog ;

namespace Idasen.BluetoothLE.Characteristics.Common
{
    [ ExcludeFromCodeCoverage ]
    [ Intercept ( typeof ( LogAspect ) ) ]
    public class BufferReader
        : IBufferReader
    {
        public BufferReader ( [ JetBrains.Annotations.NotNull ] ILogger logger )
        {
            Guard.ArgumentNotNull ( logger ,
                                    nameof ( logger ) ) ;

            _logger = logger ;
        }

        public bool TryReadValue (
            IBuffer      buffer ,
            out byte [ ] bytes )
        {
            Guard.ArgumentNotNull ( buffer ,
                                    nameof ( buffer ) ) ;

            try
            {
                var reader = DataReader.FromBuffer ( buffer ) ;
                bytes = new byte[ reader.UnconsumedBufferLength ] ;
                reader.ReadBytes ( bytes ) ;

                return true ;
            }
            catch ( Exception e )
            {
                const string message = "Failed to read from buffer" ;

                _logger.Error ( e ,
                                message ) ;
            }

            bytes = null ;

            return false ;
        }

        private readonly ILogger _logger ;
    }
}