using System ;
using Idasen.BluetoothLE.Core ;
using JetBrains.Annotations ;

namespace Idasen.BluetoothLE.Characteristics.Common
{
    public class NotInitializeException
        : Exception
    {
        public NotInitializeException ( [ NotNull ] string message )
            : base ( message )
        {
            Guard.ArgumentNotNull ( message ,
                                    nameof ( message ) ) ;
        }
    }
}