using System ;
using System.Collections.Generic ;
using System.Linq ;
using Autofac.Extras.DynamicProxy ;
using Idasen.Aop.Aspects ;
using Idasen.BluetoothLE.Characteristics.Common ;
using Idasen.BluetoothLE.Core ;
using Idasen.BluetoothLE.Linak.Interfaces ;
using JetBrains.Annotations ;
using Serilog ;

namespace Idasen.BluetoothLE.Linak
{
    [ Intercept ( typeof ( LogAspect ) ) ]
    public class RawValueToHeightAndSpeedConverter
        : IRawValueToHeightAndSpeedConverter
    {
        public RawValueToHeightAndSpeedConverter ( [ NotNull ] ILogger logger )
        {
            Guard.ArgumentNotNull ( logger ,
                                    nameof ( logger ) ) ;

            _logger = logger ;
        }

        public bool TryConvert ( IEnumerable < byte > bytes ,
                                 out uint             height ,
                                 out int              speed )
        {
            var array = bytes as byte [ ] ?? bytes.ToArray ( ) ;

            try
            {
                var rawHeight = array.Take ( 2 )
                                     .ToArray ( ) ;

                var rawSpeed = array.Skip ( 2 )
                                    .Take ( 2 )
                                    .ToArray ( ) ;

                height = HeightBaseInMicroMeter + BitConverter.ToUInt16 ( rawHeight ) ;
                speed  = BitConverter.ToInt16 ( rawSpeed ) ;

                return true ;
            }
            catch ( Exception e )
            {
                _logger.Warning ( $"Failed to convert raw value '{array.ToHex ( )}' " +
                                  $"to height and speed! ({e.Message})" ) ;

                height = 0 ;
                speed  = 0 ;

                return false ;
            }
        }

        // Height of the desk at it's lowest 620 mm and max. is 1270mm.
        internal const uint HeightBaseInMicroMeter = 6200 ; // = 6200 / 10 = 620mm

        private readonly ILogger _logger ;
    }
}