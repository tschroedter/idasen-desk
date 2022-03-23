using System.Reactive.Concurrency ;
using System.Reactive.Linq ;
using Idasen.RESTAPI.Desk.Emulator.Idasen ;
using Idasen.RESTAPI.Desk.Emulator.Interfaces ;
using Idasen.RESTAPI.MicroService.Shared.Extensions ;

namespace Idasen.RESTAPI.Desk.Emulator.Desks ;

public class RestDesk : IRestDesk
{
    private readonly IFakeDesk   _desk ;
    private readonly IDisposable _disposable ;

    private readonly ILogger _logger ;

    public RestDesk ( ILogger < RestDesk > logger ,
                      IScheduler           scheduler ,
                      IFakeDesk            desk )
    {
        _logger = logger ;
        _desk   = desk ;

        Height = 0u ;
        Speed  = 0 ;

        _disposable = _desk.HeightAndSpeedChanged
                           .ObserveOn ( scheduler )
                           .SubscribeAsync ( OnHeightAndSpeedChanged ) ;
    }

    public void Dispose ( )
    {
        _disposable.Dispose ( ) ;
        _desk.Dispose ( ) ;
    }

    public IObservable < IEnumerable < byte > > DeviceNameChanged => _desk.DeviceNameChanged ;

    public IObservable < uint > HeightChanged => _desk.HeightChanged ;

    public IObservable < int > SpeedChanged => _desk.SpeedChanged ;

    public IObservable < HeightSpeedDetails > HeightAndSpeedChanged => _desk.HeightAndSpeedChanged ;

    public IObservable < uint > FinishedChanged => _desk.FinishedChanged ;

    public IObservable < bool > RefreshedChanged => _desk.RefreshedChanged ;

    public string Name => _desk.Name ;

    public ulong BluetoothAddress => _desk.BluetoothAddress ;

    public string BluetoothAddressType => _desk.BluetoothAddressType ;

    public string DeviceName => _desk.DeviceName ;

    public void Connect ( )
    {
        _desk.Connect ( ) ;
    }

    public void MoveTo ( uint targetHeight )
    {
        _desk.MoveTo ( targetHeight ) ;
    }

    public void MoveUp ( )
    {
        _desk.MoveUp ( ) ;
    }

    public void MoveDown ( )
    {
        _desk.MoveDown ( ) ;
    }

    public void MoveStop ( )
    {
        _desk.MoveStop ( ) ;
    }

    public void MoveLock ( )
    {
        _desk.MoveLock ( ) ;
    }

    public void MoveUnlock ( )
    {
        _desk.MoveUnlock ( ) ;
    }

    public uint Height { get ; private set ; }
    public int  Speed  { get ; private set ; }

    public Task < bool > MoveToAsync ( uint targetHeight )
    {
        return DoAction ( ( ) => MoveTo ( targetHeight ) ) ;
    }

    public Task < bool > MoveUpAsync ( )
    {
        return DoAction ( MoveUp ) ;
    }

    public Task < bool > MoveDownAsync ( )
    {
        return DoAction ( MoveDown ) ;
    }

    public Task < bool > MoveStopAsync ( )
    {
        return DoAction ( MoveStop ) ;
    }

    private Task OnHeightAndSpeedChanged ( HeightSpeedDetails details )
    {
        Height = details.Height ;
        Speed  = details.Speed ;

        return Task.CompletedTask ;
    }

    private async Task < bool > DoAction ( Action action )
    {
        try
        {
            await Task.Run ( action ) ;

            return true ;
        }
        catch ( Exception e )
        {
            _logger.LogError ( e ,
                               "Failed to execute action" ) ;

            return false ;
        }
    }
}