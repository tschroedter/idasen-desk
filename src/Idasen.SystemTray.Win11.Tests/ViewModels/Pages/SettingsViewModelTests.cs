using System.Reactive.Subjects ;
using System.Reflection ;
using FluentAssertions ;
using Idasen.SystemTray.Win11.Interfaces ;
using Idasen.SystemTray.Win11.TraySettings ;
using Idasen.SystemTray.Win11.Utils ;
using Idasen.SystemTray.Win11.ViewModels.Pages ;
using Microsoft.Reactive.Testing ;
using NSubstitute ;
using Serilog ;
using System.Windows ;

namespace Idasen.SystemTray.Win11.Tests.ViewModels.Pages ;

public class SettingsViewModelTests
    : IDisposable
{
    private readonly ILogger                 _logger          = Substitute.For < ILogger > ( ) ;
    private readonly TestScheduler           _scheduler       = new( ) ;
    private readonly ILoggingSettingsManager _settingsManager = Substitute.For < ILoggingSettingsManager > ( ) ;

    private readonly Subject < ISettings >   _settingsSaved   = new( ) ;
    private readonly ISettingsSynchronizer   _synchronizer    = Substitute.For < ISettingsSynchronizer > ( ) ;
    private readonly IMainWindow             _mainWindow      = Substitute.For < IMainWindow > ( ) ;
    private readonly Subject < Visibility >  _visibilityChanges = new( ) ;

    public SettingsViewModelTests ( )
    {
        _settingsManager.SettingsFileName.Returns ( "TestSettings.json" ) ;
        _settingsManager.SettingsSaved.Returns ( _settingsSaved ) ;

        // Visibility stream for the main window
        _mainWindow.VisibilityChanged.Returns ( _visibilityChanges ) ;

        // Default synchronizer behaviors
        _synchronizer.LoadSettingsAsync ( Arg.Any < ISettingsViewModel > ( ) ,
                                          Arg.Any < CancellationToken > ( ) )
                     .Returns ( Task.CompletedTask ) ;
        _synchronizer.StoreSettingsAsync ( Arg.Any < ISettingsViewModel > ( ) ,
                                           Arg.Any < CancellationToken > ( ) )
                     .Returns ( Task.CompletedTask ) ;
    }

    public void Dispose ( )
    {
        _settingsSaved.Dispose ( ) ;
        _visibilityChanges.Dispose ( ) ;

        GC.SuppressFinalize ( this ) ;
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
    public async Task AutoSave_ShouldStore_OnSinglePropertyChange_AfterDebounce ( )
    {
        using var vm = CreateSut ( ) ;
        await vm.InitializeAsync ( CancellationToken.None ) ;

        vm.Notifications = ! vm.Notifications ;

        // Advance virtual time beyond debounce (300ms)
        _scheduler.AdvanceBy ( TimeSpan.FromMilliseconds ( 350 ).Ticks ) ;

        await _synchronizer.Received ( 1 )
                           .StoreSettingsAsync ( vm ,
                                                 Arg.Any < CancellationToken > ( ) ) ;
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
                           .StoreSettingsAsync ( vm ,
                                                 Arg.Any < CancellationToken > ( ) ) ;
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
                           .StoreSettingsAsync ( Arg.Any < ISettingsViewModel > ( ) ,
                                                 Arg.Any < CancellationToken > ( ) ) ;
    }

    [ Fact ]
    public async Task ResetSettings_ShouldCallManagerAndReload ( )
    {
        // Arrange
        var vm = CreateSut ( ) ;
        _settingsManager.ResetSettingsAsync ( Arg.Any < CancellationToken > ( ) ).Returns ( Task.CompletedTask ) ;
        _synchronizer.LoadSettingsAsync ( vm ,
                                          Arg.Any < CancellationToken > ( ) ).Returns ( Task.CompletedTask ) ;

        // Act: invoke the internal async method via reflection
        var method = typeof ( SettingsViewModel ).GetMethod ( "OnResetSettings" ,
                                                              BindingFlags.Instance | BindingFlags.NonPublic ) ;
        method.Should ( ).NotBeNull ( ) ;
        var task = ( Task ? )method.Invoke ( vm ,
                                             null ) ;
        if ( task != null ) await task ;

        // Assert
        await _settingsManager.Received ( 1 ).ResetSettingsAsync ( Arg.Any < CancellationToken > ( ) ) ;
        await _synchronizer.Received ( 1 ).LoadSettingsAsync ( vm ,
                                                               Arg.Any < CancellationToken > ( ) ) ;
    }

    [ Fact ]
    public async Task Visibility_Hidden_ShouldStoreSettings ( )
    {
        // Arrange
        var vm = CreateSut ( ) ;
        await vm.InitializeAsync ( CancellationToken.None ) ;

        // Act
        _visibilityChanges.OnNext ( Visibility.Hidden ) ;

        // Assert
        await _synchronizer.Received ( 1 )
                           .StoreSettingsAsync ( vm , Arg.Any < CancellationToken > ( ) ) ;
    }

    [ Fact ]
    public async Task Visibility_Visible_ShouldNotStoreSettings ( )
    {
        // Arrange
        var vm = CreateSut ( ) ;
        await vm.InitializeAsync ( CancellationToken.None ) ;

        // Act
        _visibilityChanges.OnNext ( Visibility.Visible ) ;

        // Assert
        await _synchronizer.DidNotReceive ( )
                           .StoreSettingsAsync ( Arg.Any < ISettingsViewModel > ( ) , Arg.Any < CancellationToken > ( ) ) ;
    }

    [ Fact ]
    public async Task SubscribeToMainWindowVisibility_ShouldNotDuplicate_Subscription ( )
    {
        // Arrange
        var vm = CreateSut ( ) ;
        await vm.InitializeAsync ( CancellationToken.None ) ;

        // Try to invoke subscription again via reflection
        var method = typeof ( SettingsViewModel ).GetMethod ( "SubscribeToMainWindowVisibility" ,
                                                              BindingFlags.Instance | BindingFlags.NonPublic ) ;
        method.Should ( ).NotBeNull ( ) ;
        method.Invoke ( vm , null ) ;

        // Act: push a single non-visible event
        _visibilityChanges.OnNext ( Visibility.Collapsed ) ;

        // Assert: should only store once (not twice)
        await _synchronizer.Received ( 1 )
                           .StoreSettingsAsync ( vm , Arg.Any < CancellationToken > ( ) ) ;
    }

    [ Fact ]
    public async Task Visibility_AfterDispose_ShouldNotStoreSettings ( )
    {
        // Arrange
        var vm = CreateSut ( ) ;
        await vm.InitializeAsync ( CancellationToken.None ) ;

        vm.Dispose ( ) ;

        // Act
        _visibilityChanges.OnNext ( Visibility.Hidden ) ;

        // Assert
        await _synchronizer.DidNotReceive ( )
                           .StoreSettingsAsync ( Arg.Any < ISettingsViewModel > ( ) , Arg.Any < CancellationToken > ( ) ) ;
    }

    private SettingsViewModel CreateSut ( )
    {
        return new SettingsViewModel ( _logger ,
                                       _settingsManager ,
                                       _scheduler ,
                                       _synchronizer ,
                                       _mainWindow ) ;
    }
}