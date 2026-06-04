using Idasen.BluetoothLE.Linak.Interfaces ;
using Idasen.SystemTray.Win11.Interfaces ;
using Serilog ;

namespace Idasen.SystemTray.Win11.Utils ;

public sealed class DeskMovementManager : IDeskMovementManager
{
    private const uint DeskHeightFactor = 100 ;

    private readonly ILogger             _logger ;
    private readonly ISettingsManager    _settingsManager ;
    private Func < IDesk ? > ?           _deskAccessor ;

    public DeskMovementManager (
        ILogger          logger ,
        ISettingsManager settingsManager )
    {
        ArgumentNullException.ThrowIfNull ( logger ) ;
        ArgumentNullException.ThrowIfNull ( settingsManager ) ;

        _logger           = logger ;
        _settingsManager  = settingsManager ;
    }

    public void SetDeskAccessor ( Func < IDesk ? > deskAccessor )
    {
        ArgumentNullException.ThrowIfNull ( deskAccessor ) ;
        _deskAccessor = deskAccessor ;
    }

    public bool IsDeskAvailable ( )
    {
        return _deskAccessor?.Invoke ( ) is not null ;
    }

    public async Task MoveToHeightAsync ( uint heightInCentimeters , string operationName )
    {
        _logger.Debug ( "Executing {OperationName} to move desk to {HeightInCm} cm..." ,
                        operationName ,
                        heightInCentimeters ) ;

        var desk = _deskAccessor?.Invoke ( ) ;
        if ( desk is null )
        {
            _logger.Warning ( "{OperationName} failed - desk is not connected" ,
                             operationName ) ;
            return ;
        }

        await _settingsManager.LoadAsync ( CancellationToken.None ).ConfigureAwait ( false ) ;

        var heightInMillimeters = HeightToDeskHeight ( heightInCentimeters ) ;

        _logger.Information ( "Moving desk to {HeightInCm} cm ({HeightInMm} mm)..." ,
                              heightInCentimeters ,
                              heightInMillimeters ) ;

        desk.MoveTo ( heightInMillimeters ) ;
    }

    private static uint HeightToDeskHeight ( uint heightInCm )
    {
        return heightInCm * DeskHeightFactor ;
    }
}
