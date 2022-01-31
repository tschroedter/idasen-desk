using JetBrains.Annotations ;

namespace Idasen.BluetoothLE.Linak.Interfaces
{
    public interface IDeskLockerFactory
    {
        IDeskLocker Create ( [ NotNull ] IDeskMover           deskMover ,
                             [NotNull]   IDeskCommandExecutor executer ,
                             [NotNull]   IDeskHeightAndSpeed  heightAndSpeed ) ;
    }
}