using Idasen.BluetoothLE.Linak.Control ;
using JetBrains.Annotations ;

namespace Idasen.BluetoothLE.Linak.Interfaces
{
    public interface IInitialHeightAndSpeedProviderFactory
    {
        IInitialHeightProvider Create ( [ NotNull ] IDeskCommandExecutor executor ,
                                        [ NotNull ] IDeskHeightAndSpeed  heightAndSpeed ) ;
    }
}