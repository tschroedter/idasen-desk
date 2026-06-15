using System.Reactive.Concurrency ;
using System.Reactive.Subjects ;
using FluentAssertions ;
using Idasen.BluetoothLE.Linak.Interfaces ;
using Idasen.SystemTray.Win11.Interfaces ;
using Idasen.SystemTray.Win11.TraySettings ;
using Idasen.SystemTray.Win11.Utils ;
using NSubstitute ;
using Serilog ;
using Wpf.Ui.Controls ;
using Wpf.Ui.Tray.Controls ;

namespace Idasen.SystemTray.Win11.Tests.Utils ;

public sealed class DeskNotificationManagerTests : IDisposable
{
    private readonly ILogger                _logger ;
    private readonly INotifications         _notifications ;
    private readonly IErrorManager          _errorManager ;
    private readonly ISettingsManager       _settingsManager ;
    private readonly IStatusBarManager      _statusBarManager ;
    private readonly IScheduler             _scheduler ;
    private readonly IDeskConnectionManager _deskConnectionManager ;
    private readonly Subject < IErrorDetails > _errorChangedSubject ;
    private readonly Subject < StatusBarInfo > _statusBarInfoSubject ;

    public DeskNotificationManagerTests ( )
    {
        _logger                = Substitute.For < ILogger > ( ) ;
        _notifications         = Substitute.For < INotifications > ( ) ;
        _errorManager          = Substitute.For < IErrorManager > ( ) ;
        _settingsManager       = Substitute.For < ISettingsManager > ( ) ;
        _statusBarManager      = Substitute.For < IStatusBarManager > ( ) ;
        _scheduler             = Scheduler.Immediate ;
        _deskConnectionManager = Substitute.For < IDeskConnectionManager > ( ) ;

        _errorChangedSubject = new Subject < IErrorDetails > ( ) ;
        _errorManager.ErrorChanged.Returns ( _errorChangedSubject ) ;

        _statusBarInfoSubject = new Subject < StatusBarInfo > ( ) ;
        _statusBarManager.StatusBarInfoChanged.Returns ( _statusBarInfoSubject ) ;

        var settings = new Settings
        {
            HeightSettings = new HeightSettings
            {
                LastKnownDeskHeight = 1000
            }
        } ;
        _settingsManager.CurrentSettings.Returns ( settings ) ;
    }

    public void Dispose ( )
    {
        _errorChangedSubject.Dispose ( ) ;
        _statusBarInfoSubject.Dispose ( ) ;
    }

    [ Fact ]
    public void Constructor_NullLogger_ThrowsArgumentNullException ( )
    {
        // Act
        var act = ( ) => new DeskNotificationManager (
            null ! ,
            _notifications ,
            _errorManager ,
            _settingsManager ,
            _statusBarManager ,
            _scheduler ,
            _deskConnectionManager ) ;

        // Assert
        act.Should ( ).Throw < ArgumentNullException > ( )
           .WithParameterName ( "logger" ) ;
    }

    [ Fact ]
    public void Constructor_NullNotifications_ThrowsArgumentNullException ( )
    {
        // Act
        var act = ( ) => new DeskNotificationManager (
            _logger ,
            null ! ,
            _errorManager ,
            _settingsManager ,
            _statusBarManager ,
            _scheduler ,
            _deskConnectionManager ) ;

        // Assert
        act.Should ( ).Throw < ArgumentNullException > ( )
           .WithParameterName ( "notifications" ) ;
    }

    [ Fact ]
    public void Constructor_NullErrorManager_ThrowsArgumentNullException ( )
    {
        // Act
        var act = ( ) => new DeskNotificationManager (
            _logger ,
            _notifications ,
            null ! ,
            _settingsManager ,
            _statusBarManager ,
            _scheduler ,
            _deskConnectionManager ) ;

        // Assert
        act.Should ( ).Throw < ArgumentNullException > ( )
           .WithParameterName ( "errorManager" ) ;
    }

    [ Fact ]
    public void Constructor_NullSettingsManager_ThrowsArgumentNullException ( )
    {
        // Act
        var act = ( ) => new DeskNotificationManager (
            _logger ,
            _notifications ,
            _errorManager ,
            null ! ,
            _statusBarManager ,
            _scheduler ,
            _deskConnectionManager ) ;

        // Assert
        act.Should ( ).Throw < ArgumentNullException > ( )
           .WithParameterName ( "settingsManager" ) ;
    }

    [ Fact ]
    public void Constructor_NullStatusBarManager_ThrowsArgumentNullException ( )
    {
        // Act
        var act = ( ) => new DeskNotificationManager (
            _logger ,
            _notifications ,
            _errorManager ,
            _settingsManager ,
            null ! ,
            _scheduler ,
            _deskConnectionManager ) ;

        // Assert
        act.Should ( ).Throw < ArgumentNullException > ( )
           .WithParameterName ( "statusBarManager" ) ;
    }

    [ Fact ]
    public void Constructor_NullScheduler_ThrowsArgumentNullException ( )
    {
        // Act
        var act = ( ) => new DeskNotificationManager (
            _logger ,
            _notifications ,
            _errorManager ,
            _settingsManager ,
            _statusBarManager ,
            null ! ,
            _deskConnectionManager ) ;

        // Assert
        act.Should ( ).Throw < ArgumentNullException > ( )
           .WithParameterName ( "scheduler" ) ;
    }

    [ Fact ]
    public void Constructor_NullDeskConnectionManager_ThrowsArgumentNullException ( )
    {
        // Act
        var act = ( ) => new DeskNotificationManager (
            _logger ,
            _notifications ,
            _errorManager ,
            _settingsManager ,
            _statusBarManager ,
            _scheduler ,
            null ! ) ;

        // Assert
        act.Should ( ).Throw < ArgumentNullException > ( )
           .WithParameterName ( "deskConnectionManager" ) ;
    }

    [ Fact ]
    public void Constructor_ValidParameters_CreatesInstance ( )
    {
        // Act
        var manager = new DeskNotificationManager (
            _logger ,
            _notifications ,
            _errorManager ,
            _settingsManager ,
            _statusBarManager ,
            _scheduler ,
            _deskConnectionManager ) ;

        // Assert
        manager.Should ( ).NotBeNull ( ) ;
    }

    [ Fact ]
    public void StatusBarInfoChanged_ReturnsStatusBarManagerObservable ( )
    {
        // Arrange
        var manager = new DeskNotificationManager (
            _logger ,
            _notifications ,
            _errorManager ,
            _settingsManager ,
            _statusBarManager ,
            _scheduler ,
            _deskConnectionManager ) ;

        // Act
        var observable = manager.StatusBarInfoChanged ;

        // Assert
        observable.Should ( ).BeSameAs ( _statusBarManager.StatusBarInfoChanged ) ;
    }

    [ Fact ]
    public void Initialize_CallsNotificationsInitialize ( )
    {
        // Arrange
        var manager = CreateManager ( ) ;
        var token   = CancellationToken.None ;

        // We can't create NotifyIcon in a non-STA thread, so we just pass null
        // The implementation should handle this gracefully
        _notifications.When ( x => x.Initialize ( Arg.Any < NotifyIcon > ( ) , Arg.Any < CancellationToken > ( ) ) )
                     .Do ( _ => { } ) ; // Just accept the call

        // Act & Assert - Should not throw even with null NotifyIcon
        var act = ( ) => manager.Initialize ( null ! , token ) ;
        act.Should ( ).NotThrow ( ) ;

        _notifications.Received ( 1 ).Initialize ( null ! , token ) ;
    }

    [ Fact ]
    public void Initialize_SubscribesToErrorChanged ( )
    {
        // Arrange
        var manager = CreateManager ( ) ;
        var token   = CancellationToken.None ;

        _notifications.When ( x => x.Initialize ( Arg.Any < NotifyIcon > ( ) , Arg.Any < CancellationToken > ( ) ) )
                     .Do ( _ => { } ) ;

        // Act
        manager.Initialize ( null ! , token ) ;

        // Assert - Verify subscription by triggering an error
        var errorDetails = Substitute.For < IErrorDetails > ( ) ;
        errorDetails.Message.Returns ( "Test error" ) ;

        _errorChangedSubject.OnNext ( errorDetails ) ;

        _statusBarManager.Received ( 1 ).UpdateStatus ( Arg.Any < StatusBarInfo > ( ) ) ;
    }

    [ Fact ]
    public void ShowNotification_CallsNotificationsShow ( )
    {
        // Arrange
        var manager  = CreateManager ( ) ;
        var title    = "Test Title" ;
        var message  = "Test Message" ;
        var severity = InfoBarSeverity.Informational ;

        // Act
        manager.ShowNotification ( title , message , severity ) ;

        // Assert
        _notifications.Received ( 1 ).Show ( title , message , severity ) ;
    }

    [ Fact ]
    public void ShowStatusUpdate_WithNonZeroHeight_UpdatesStatusBarAndShowsNotification ( )
    {
        // Arrange
        var manager  = CreateManager ( ) ;
        var height   = 1200u ;
        var title    = "Status" ;
        var message  = "Height updated" ;
        var severity = InfoBarSeverity.Success ;

        // Act
        manager.ShowStatusUpdate ( height , title , message , severity ) ;

        // Assert
        _statusBarManager.Received ( 1 ).UpdateStatus ( Arg.Is < StatusBarInfo > ( info =>
            info.Title == title &&
            info.Height == height &&
            info.Message == message &&
            info.Severity == severity ) ) ;

        _notifications.Received ( 1 ).Show ( title , message , severity ) ;
    }

    [ Fact ]
    public void ShowStatusUpdate_WithZeroHeight_UsesLastKnownHeight ( )
    {
        // Arrange
        var manager  = CreateManager ( ) ;
        var height   = 0u ;
        var title    = "Status" ;
        var message  = "Using last known height" ;
        var severity = InfoBarSeverity.Warning ;

        // Act
        manager.ShowStatusUpdate ( height , title , message , severity ) ;

        // Assert
        _statusBarManager.Received ( 1 ).UpdateStatus ( Arg.Is < StatusBarInfo > ( info =>
            info.Height == 1000 ) ) ; // LastKnownDeskHeight from setup

        _notifications.Received ( 1 ).Show ( title , message , severity ) ;
    }

    [ Fact ]
    public void ErrorChanged_TriggersOnErrorChanged ( )
    {
        // Arrange
        var manager = CreateManager ( ) ;
        manager.Initialize ( null ! , CancellationToken.None ) ;

        var desk = Substitute.For < IDesk > ( ) ;
        desk.DeviceName.Returns ( "TestDesk" ) ;
        _deskConnectionManager.CurrentDesk.Returns ( desk ) ;

        var errorDetails = Substitute.For < IErrorDetails > ( ) ;
        errorDetails.Message.Returns ( "Test error message" ) ;

        // Act
        _errorChangedSubject.OnNext ( errorDetails ) ;

        // Assert
        _statusBarManager.Received ( 1 ).UpdateStatus ( Arg.Is < StatusBarInfo > ( info =>
            info.Title == "Error" &&
            info.Message.Contains ( "TestDesk" ) &&
            info.Message.Contains ( "Test error message" ) &&
            info.Severity == InfoBarSeverity.Error ) ) ;
    }

    [ Fact ]
    public void ErrorChanged_WhenCurrentDeskIsNull_UsesUnknownDeviceName ( )
    {
        // Arrange
        var manager = CreateManager ( ) ;
        manager.Initialize ( null ! , CancellationToken.None ) ;

        _deskConnectionManager.CurrentDesk.Returns ( ( IDesk ? )null ) ;

        var errorDetails = Substitute.For < IErrorDetails > ( ) ;
        errorDetails.Message.Returns ( "Test error" ) ;

        // Act
        _errorChangedSubject.OnNext ( errorDetails ) ;

        // Assert
        _statusBarManager.Received ( 1 ).UpdateStatus ( Arg.Is < StatusBarInfo > ( info =>
            info.Message.Contains ( "Unknown" ) &&
            info.Message.Contains ( "Test error" ) ) ) ;
    }

    [ Fact ]
    public void ErrorChanged_CallsShowStatusUpdateWithCorrectParameters ( )
    {
        // Arrange
        var manager = CreateManager ( ) ;
        manager.Initialize ( null ! , CancellationToken.None ) ;

        var errorDetails = Substitute.For < IErrorDetails > ( ) ;
        errorDetails.Message.Returns ( "Connection failed" ) ;

        // Act
        _errorChangedSubject.OnNext ( errorDetails ) ;

        // Assert
        _statusBarManager.Received ( 1 ).UpdateStatus ( Arg.Is < StatusBarInfo > ( info =>
            info.Height == 1000 && // LastKnownDeskHeight
            info.Title == "Error" &&
            info.Severity == InfoBarSeverity.Error ) ) ;

        _notifications.Received ( 1 ).Show (
            "Error" ,
            Arg.Any < string > ( ) ,
            InfoBarSeverity.Error ) ;
    }

    [ Fact ]
    public void Dispose_WhenNotDisposed_DisposesErrorChangedSubscription ( )
    {
        // Arrange
        var manager = CreateManager ( ) ;
        manager.Initialize ( null ! , CancellationToken.None ) ;

        // Act
        manager.Dispose ( ) ;

        // Assert - After disposal, error events should not trigger handlers
        var errorDetails = Substitute.For < IErrorDetails > ( ) ;
        errorDetails.Message.Returns ( "Should not process" ) ;

        _statusBarManager.ClearReceivedCalls ( ) ;
        _errorChangedSubject.OnNext ( errorDetails ) ;

        _statusBarManager.DidNotReceive ( ).UpdateStatus ( Arg.Any < StatusBarInfo > ( ) ) ;
    }

    [ Fact ]
    public void Dispose_MultipleTimes_DoesNotThrow ( )
    {
        // Arrange
        var manager = CreateManager ( ) ;

        // Act
        var act = ( ) =>
        {
            manager.Dispose ( ) ;
            manager.Dispose ( ) ;
            manager.Dispose ( ) ;
        } ;

        // Assert
        act.Should ( ).NotThrow ( ) ;
    }

    [ Fact ]
    public void Dispose_WhenInitialized_DoesNotThrow ( )
    {
        // Arrange
        var manager = CreateManager ( ) ;
        manager.Initialize ( null ! , CancellationToken.None ) ;

        // Act
        var act = ( ) => manager.Dispose ( ) ;

        // Assert
        act.Should ( ).NotThrow ( ) ;
    }

    [ Fact ]
    public void Dispose_WhenNotInitialized_DoesNotThrow ( )
    {
        // Arrange
        var manager = CreateManager ( ) ;

        // Act
        var act = ( ) => manager.Dispose ( ) ;

        // Assert
        act.Should ( ).NotThrow ( ) ;
    }

    private DeskNotificationManager CreateManager ( )
    {
        return new DeskNotificationManager (
            _logger ,
            _notifications ,
            _errorManager ,
            _settingsManager ,
            _statusBarManager ,
            _scheduler ,
            _deskConnectionManager ) ;
    }
}
