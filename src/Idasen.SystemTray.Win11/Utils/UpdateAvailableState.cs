using System.Reactive.Subjects ;
using Idasen.SystemTray.Win11.Interfaces ;

namespace Idasen.SystemTray.Win11.Utils ;

public sealed class UpdateAvailableState : IUpdateAvailableState
{
    private readonly BehaviorSubject < bool > _updateAvailableChanged = new ( false ) ;

    public bool IsUpdateAvailable { get ; private set ; }

    public IObservable < bool > UpdateAvailableChanged => _updateAvailableChanged ;

    public void SetUpdateAvailable ( bool updateAvailable )
    {
        if ( IsUpdateAvailable == updateAvailable )
            return ;

        IsUpdateAvailable = updateAvailable ;
        _updateAvailableChanged.OnNext ( updateAvailable ) ;
    }

    public void Dispose ( )
    {
        _updateAvailableChanged.Dispose ( ) ;
    }
}
