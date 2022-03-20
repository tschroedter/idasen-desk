using Idasen.RESTAPI.Desk.Emulator.Idasen ;

namespace Idasen.RESTAPI.Desk.Emulator.Interfaces ;

public interface IRestDesk : IDesk
{
    uint          Height { get ; }
    int           Speed  { get ; }
    Task < bool > MoveToAsync ( uint targetHeight ) ;
    Task < bool > MoveUpAsync ( ) ;
    Task < bool > MoveDownAsync ( ) ;
    Task < bool > MoveStopAsync ( ) ;
}