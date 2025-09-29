using System.Reactive.Subjects ;
using FluentAssertions ;
using Idasen.SystemTray.Win11.Interfaces ;
using Idasen.SystemTray.Win11.TraySettings ;
using Idasen.SystemTray.Win11.Utils ;
using Idasen.SystemTray.Win11.ViewModels.Pages ;
using NSubstitute ;
using Serilog ;
using Wpf.Ui.Controls ;
using Microsoft.Reactive.Testing ;
using System.Reflection ;

namespace Idasen.SystemTray.Win11.Tests.ViewModels.Pages ;

public class SettingsViewModelTests
    : IDisposable
{
    private readonly ILogger                 _logger          = Substitute.For < ILogger > ( ) ;
    private readonly TestScheduler           _scheduler       = new ( ) ;
    private readonly ILoggingSettingsManager _settingsManager = Substitute.For < ILoggingSettingsManager > ( ) ;

    private readonly Subject < ISettings >     _settingsSaved = new ( ) ;
    private readonly Subject < StatusBarInfo > _statusSubject = new ( ) ;
    private readonly ISettingsSynchronizer     _synchronizer  = Substitute.For < ISettingsSynchronizer > ( ) ;
    private readonly ITimer                    _timer         = Substitute.For < ITimer > ( ) ;
    private readonly IUiDeskManager            _uiDeskManager = Substitute.For < IUiDeskManager > ( ) ;

    public SettingsViewModelTests ( )
    {
        _settingsManager.SettingsFileName.Returns ( "TestSettings.json" ) ;
        _settingsManager.SettingsSaved.Returns ( _settingsSaved ) ;

        _uiDeskManager.StatusBarInfoChanged.Returns ( _statusSubject ) ;
        _uiDeskManager.LastStatusBarInfo.Returns ( new StatusBarInfo ( "Initial Title" ,
                                                                       100 ,
                                                                       "Initial Message" ,
                                                                       InfoBarSeverity.Informational ) ) ;

        // Default synchronizer behaviors
        _synchronizer.LoadSettingsAsync ( Arg.Any < ISettingsViewModel > ( ) , Arg.Any < CancellationToken > ( ) )
                     .Returns ( Task.CompletedTask ) ;
        _synchronizer.StoreSettingsAsync ( Arg.Any < ISettingsViewModel > ( ) , Arg.Any < CancellationToken > ( ) )
                     .Returns ( Task.CompletedTask ) ;
    }

    [ Fact ]
    public async Task InitializeAsync_ShouldLoadSettings_And_SubscribeToSettingsSaved ( )
    {
        // Arrange
        var vm = CreateSut ( ) ;

        // Act
        await vm.InitializeAsync ( CancellationToken.None ) ;

        // Assert
        await _synchronizer.Received ( 1 )
                           .LoadSettingsAsync ( vm ,
                                                CancellationToken.None ) ;

        // simulate settings saved event and verify ViewModel updates
        var s = new Settings { HeightSettings = { LastKnownDeskHeight = 150 } } ;
        _settingsSaved.OnNext ( s ) ;

        // Pump scheduled work (ObserveOn(TestScheduler)) so the subscription runs
        _scheduler.Start ( ) ;

        vm.LastKnownDeskHeight.Should ( ).Be ( 150 ) ;
    }

    [ Fact ]
    public async Task InitializeAsync_ShouldSetInfoBar_FromLastStatusBarInfo ( )
    {
        // Arrange
        var vm = CreateSut ( ) ;

        // Act
        await vm.InitializeAsync ( CancellationToken.None ) ;

        // Assert
        vm.Message.Should ( ).Be ( "Initial Message" ) ;
        vm.Severity.Should ( ).Be ( InfoBarSeverity.Informational ) ;
    }

    [ Fact ]
    public async Task OnStatusBarInfoChanged_ShouldUpdateProperties_AndResetTimer ( )
    {
        // Arrange
        var vm = CreateSut ( ) ;
        await vm.InitializeAsync ( CancellationToken.None ) ;

        // Act
        var newInfo = new StatusBarInfo ( "Updated Title" ,
                                          120 ,
                                          "Updated Message" ,
                                          InfoBarSeverity.Warning ) ;
        _statusSubject.OnNext ( newInfo ) ;

        // Pump scheduled work
        _scheduler.Start ( ) ;

        // Assert
        vm.Message.Should ( ).Be ( "Updated Message" ) ;
        vm.Severity.Should ( ).Be ( InfoBarSeverity.Warning ) ;
        vm.Height.Should ( ).Be ( 120 ) ;
        // Timer should be reset to 10s
        _timer.Received ( 1 ).Change ( TimeSpan.FromSeconds ( 10 ) ,
                                       Timeout.InfiniteTimeSpan ) ;
    }

    [ Fact ]
    public void DefaultInfoBar_ShouldSetMessageAndSeverity_WhenHeightIsZero_AndConnected ( )
    {
        // Arrange
        var vm = CreateSut ( ) ;
        _uiDeskManager.IsConnected.Returns ( true ) ;
        vm.Height = 0 ;

        // Act
        vm.DefaultInfoBar ( ) ;

        // Assert
        vm.Message.Should ( ).Be ( "Can't determine desk height." ) ;
        vm.Severity.Should ( ).Be ( InfoBarSeverity.Informational ) ;
    }

    [ Fact ]
    public void DefaultInfoBar_ShouldSetMessageAndSeverity_WhenHeightIsNonZero_AndConnected ( )
    {
        // Arrange
        var vm = CreateSut ( ) ;
        _uiDeskManager.IsConnected.Returns ( true ) ;
        vm.Height = 123 ;

        // Act
        vm.DefaultInfoBar ( ) ;

        // Assert
        vm.Message.Should ( ).Be ( "Current desk height 123 cm" ) ;
        vm.Severity.Should ( ).Be ( InfoBarSeverity.Informational ) ;
    }

    [ Fact ]
    public void DefaultInfoBar_ShouldNotUpdate_WhenManagerIsNotConnected ( )
    {
        // Arrange
        var vm = CreateSut ( ) ;
        _uiDeskManager.IsConnected.Returns ( false ) ;

        // Act
        vm.DefaultInfoBar ( ) ;

        // Assert
        vm.Message.Should ( ).Be ( "Unknown" ) ;
        vm.Severity.Should ( ).Be ( InfoBarSeverity.Informational ) ;
    }

    [ Fact ]
    public async Task OnNavigatedFromAsync_ShouldStoreSettings ( )
    {
        // Arrange
        var vm = CreateSut ( ) ;

        // Act
        await vm.OnNavigatedFromAsync ( ) ;

        // Assert
        await _synchronizer.Received ( 1 )
                           .StoreSettingsAsync ( vm ,
                                                 CancellationToken.None ) ;
    }

    [ Fact ]
    public async Task Dispose_ShouldDisposeTimer ( )
    {
        // Arrange
        var vm = CreateSut ( ) ;
        await vm.InitializeAsync ( CancellationToken.None ) ;

        // Act
        vm.Dispose ( ) ;

        // Assert
        _timer.Received ( 1 ).Dispose ( ) ;
    }

    [ Fact ]
    public async Task AutoSave_ShouldStore_OnSinglePropertyChange_AfterDebounce ( )
    {
        using var vm = CreateSut ( ) ;
        await vm.InitializeAsync ( CancellationToken.None ) ;

        vm.Notifications = ! vm.Notifications ;

        // Advance virtual time beyond debounce (300ms)
        _scheduler.AdvanceBy ( TimeSpan.FromMilliseconds ( 350 ).Ticks ) ;

        await _synchronizer.Received ( 1 )
                           .StoreSettingsAsync ( vm , Arg.Any < CancellationToken > ( ) ) ;
    }

    [ Fact ]
    public async Task AutoSave_ShouldCoalesce_MultipleRapidChanges ( )
    {
        using var vm = CreateSut ( ) ;
        await vm.InitializeAsync ( CancellationToken.None ) ;

        vm.Notifications = ! vm.Notifications ;
        // within debounce window, change again
        _scheduler.AdvanceBy ( TimeSpan.FromMilliseconds ( 100 ).Ticks ) ;
        vm.Notifications = ! vm.Notifications ;
        _scheduler.AdvanceBy ( TimeSpan.FromMilliseconds ( 100 ).Ticks ) ;
        vm.ParentalLock = ! vm.ParentalLock ;

        // Now advance past debounce window from last change
        _scheduler.AdvanceBy ( TimeSpan.FromMilliseconds ( 350 ).Ticks ) ;

        await _synchronizer.Received ( 1 )
                           .StoreSettingsAsync ( vm , Arg.Any < CancellationToken > ( ) ) ;
    }

    [ Fact ]
    public async Task AutoSave_ShouldNotRun_AfterDispose ( )
    {
        var vm = CreateSut ( ) ;
        await vm.InitializeAsync ( CancellationToken.None ) ;
        vm.Dispose ( ) ;

        vm.Notifications = ! vm.Notifications ;
        _scheduler.AdvanceBy ( TimeSpan.FromMilliseconds ( 400 ).Ticks ) ;

        await _synchronizer.DidNotReceive ( )
                           .StoreSettingsAsync ( Arg.Any < ISettingsViewModel > ( ) , Arg.Any < CancellationToken > ( ) ) ;
    }

    [ Fact ]
    public async Task ResetSettings_ShouldCallManagerAndReload ( )
    {
        // Arrange
        var vm = CreateSut ( ) ;
        _settingsManager.ResetSettingsAsync ( Arg.Any<CancellationToken> ( ) ).Returns ( Task.CompletedTask ) ;
        _synchronizer.LoadSettingsAsync ( vm , Arg.Any<CancellationToken> ( ) ).Returns ( Task.CompletedTask ) ;

        // Act: invoke the internal async method via reflection
        var method = typeof ( SettingsViewModel ).GetMethod ( "OnResetSettings" , BindingFlags.Instance | BindingFlags.NonPublic ) ;
        method.Should ( ).NotBeNull ( ) ;
        var task = ( Task? ) method.Invoke ( vm , null ) ;
        if ( task != null ) await task ;

        // Assert
        await _settingsManager.Received ( 1 ).ResetSettingsAsync ( Arg.Any<CancellationToken> ( ) ) ;
        await _synchronizer.Received ( 1 ).LoadSettingsAsync ( vm , Arg.Any<CancellationToken> ( ) ) ;
    }

    private SettingsViewModel CreateSut ( )
    {
        return new SettingsViewModel ( _logger ,
                                       _settingsManager ,
                                       _scheduler ,
                                       _synchronizer ,
                                       _uiDeskManager ,
                                       TimerFactory ) ;
    }

    private ITimer TimerFactory ( TimerCallback callback , object ? state , TimeSpan dueTime , TimeSpan period )
    {
        // We don't need to simulate the timer's callback in these tests;
        // we only assert that Change and Dispose are called.
        return _timer ;
    }

    public void Dispose ( )
    {
        _settingsSaved.Dispose ( ) ;
        _statusSubject.Dispose ( ) ;

        GC.SuppressFinalize ( this );
    }
}