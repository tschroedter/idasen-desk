using System ;
using JetBrains.Annotations ;

namespace Idasen.BluetoothLE.Linak.Interfaces
{
    public interface IErrorDetails
    {
        [ NotNull ] string Message { get ; }

        [ CanBeNull ] Exception Exception { get ; }
        string                  Caller    { get ; }
    }
}