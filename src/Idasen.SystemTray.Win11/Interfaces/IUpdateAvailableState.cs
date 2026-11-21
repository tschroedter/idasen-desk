namespace Idasen.SystemTray.Win11.Interfaces ;

public interface IUpdateAvailableState : IDisposable
{
    bool IsUpdateAvailable { get ; }

    IObservable < bool > UpdateAvailableChanged { get ; }

    void SetUpdateAvailable ( bool updateAvailable ) ;
}
