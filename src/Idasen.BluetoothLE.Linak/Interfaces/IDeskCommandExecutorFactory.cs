using Idasen.BluetoothLE.Characteristics.Interfaces.Characteristics ;
using JetBrains.Annotations ;

namespace Idasen.BluetoothLE.Linak.Interfaces
{
    public interface IDeskCommandExecutorFactory
    {
        IDeskCommandExecutor Create ( [ NotNull ] IControl control ) ;
    }
}