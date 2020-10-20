using System ;
using System.Collections.Generic ;
using System.Linq ;
using Idasen.BluetoothLE.Characteristics.Common ;
using Idasen.BluetoothLE.Core ;
using JetBrains.Annotations ;
using Serilog ;

namespace Idasen.BluetoothLE.Characteristics.Characteristics
{
    public class RawBytesToHeightAndSpeedConverter  // todo testing
        : IRawBytesToHeightAndSpeedConverter
    {
        private readonly ILogger _logger ;

        public RawBytesToHeightAndSpeedConverter ( [ NotNull ] ILogger logger)
        {
            Guard.ArgumentNotNull ( logger ,
                                    nameof ( logger ) ) ;

            _logger = logger ;
        }

        public bool TryConvert(IEnumerable<byte> bytes,
                               out uint          height,
                               out int           speed)
        {
            var array = bytes as byte[] ?? bytes.ToArray();

            try
            {
                var rawHeight = array.Take(2)
                                     .ToArray();

                var rawSpeed = array.Skip(2)
                                    .Take(2)
                                    .ToArray();

                height = 6200u + BitConverter.ToUInt16(rawHeight);
                speed  = BitConverter.ToInt16(rawSpeed);

                return true;
            }
            catch (Exception e)
            {
                _logger.Warning($"Failed to convert raw value '{array.ToHex()}' " +
                                $"to height and speed! ({e.Message})");

                height = 0;
                speed  = 0;

                return false;
            }
        }
    }
}