using Idasen.BluetoothLE.Linak.Interfaces ;

namespace Idasen.SystemTray.Win11.Interfaces ;

public interface IDeskMovementManager
{
    Task MoveToHeightAsync ( uint heightInCentimeters , string operationName ) ;
    bool IsDeskAvailable ( ) ;
}
