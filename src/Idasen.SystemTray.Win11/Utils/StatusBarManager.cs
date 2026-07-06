using System.Reactive.Subjects ;
using Idasen.SystemTray.Win11.Interfaces ;
using Serilog ;
using Wpf.Ui.Controls ;

namespace Idasen.SystemTray.Win11.Utils ;

public sealed class StatusBarManager : IStatusBarManager , IDisposable
{
    private readonly ILogger                   _logger ;
    private readonly Subject < StatusBarInfo > _statusBarInfoSubject ;
    private          bool                      _disposed ;

    public StatusBarManager ( ILogger logger )
    {
        ArgumentNullException.ThrowIfNull ( logger ) ;

        _logger               = logger ;
        _statusBarInfoSubject = new Subject < StatusBarInfo > ( ) ;
    }

    public void Dispose ( )
    {
        if ( _disposed )
            return ;

        _disposed = true ;

        try
        {
            _statusBarInfoSubject.OnCompleted ( ) ;
            _statusBarInfoSubject.Dispose ( ) ;
        }
        catch ( Exception ex )
        {
            _logger.Warning ( ex ,
                              "Failed to dispose {ResourceName}" ,
                              nameof ( _statusBarInfoSubject ) ) ;
        }
    }

    public IObservable < StatusBarInfo > StatusBarInfoChanged => _statusBarInfoSubject ;

    public void UpdateStatus ( StatusBarInfo info )
    {
        ArgumentNullException.ThrowIfNull ( info ) ;

        _logger.Debug ( "Updating status bar: {Status}" ,
                        info ) ;

        _statusBarInfoSubject.OnNext ( info ) ;
    }

    public void UpdateDeskHeight ( uint heightInMillimeters )
    {
        var heightInCm = heightInMillimeters / AppConfiguration.UI.DeskHeightFactor ;

        _logger.Debug ( "Updating desk height status: {Height}cm" ,
                        heightInCm ) ;

        var info = new StatusBarInfo (
                                      "" ,
                                      heightInMillimeters ,
                                      $"Height: {heightInCm} cm" ,
                                      InfoBarSeverity.Informational ) ;

        _statusBarInfoSubject.OnNext ( info ) ;
    }
}
