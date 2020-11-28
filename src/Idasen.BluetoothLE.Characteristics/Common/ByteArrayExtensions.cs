using System ;
using System.Collections.Generic ;
using System.Linq ;
using Idasen.BluetoothLE.Core ;
using JetBrains.Annotations ;

namespace Idasen.BluetoothLE.Characteristics.Common
{
    public static class ByteArrayExtensions
    {
        public static string ToHex ( [ NotNull ] this IEnumerable < byte > array )
        {
            Guard.ArgumentNotNull ( array ,
                                    nameof ( array ) ) ;

            return BitConverter.ToString ( array.ToArray ( ) ) ; //.Replace("-", "");
        }
    }
}