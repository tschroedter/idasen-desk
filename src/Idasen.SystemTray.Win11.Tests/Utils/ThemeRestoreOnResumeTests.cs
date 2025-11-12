using System.Reflection ;
using FluentAssertions ;
using Idasen.SystemTray.Win11.Interfaces ;
using Idasen.SystemTray.Win11.Utils ;
using Microsoft.Win32 ;
using NSubstitute ;
using Serilog ;

namespace Idasen.SystemTray.Win11.Tests.Utils ;

public class ThemeRestoreOnResumeTests
{
    private readonly ILogger      _logger      = Substitute.For < ILogger > ( ) ;
    private readonly IPowerEvents _powerEvents = Substitute.For < IPowerEvents > ( ) ;

    private readonly IThemeRestoreWithDelayOnResume _themeRestore =
        Substitute.For < IThemeRestoreWithDelayOnResume > ( ) ;

    public ThemeRestoreOnResumeTests ( )
    {
        _themeRestore.ApplyWithDelayAsync ( Arg.Any < CancellationToken > ( ) ).Returns ( Task.CompletedTask ) ;
    }

    [ Fact ]
    public async Task PowerModeChanged_ShouldTriggerThemeRestore ( )
    {
        // Arrange
        using var sut = CreateSut ( ) ;

        sut.Initialize ( ) ;

        // Act - raise event on substitute
        _powerEvents.PowerModeChanged += Raise.Event < PowerModeChangedEventHandler > ( this ,
                                                                                        new
                                                                                            PowerModeChangedEventArgs ( PowerModes
                                                                                                                           .Resume ) ) ;

        // Allow background task to run
        await Task.Delay ( 20 ) ;

        // Assert
        await _themeRestore.Received ( 1 ).ApplyWithDelayAsync ( Arg.Any < CancellationToken > ( ) ) ;
    }

    private ThemeRestoreOnResume CreateSut ( )
    {
        return new ThemeRestoreOnResume ( _logger ,
                                          _powerEvents ,
                                          _themeRestore ) ;
    }

    [ Fact ]
    public async Task DisplaySettingsChanged_ShouldTriggerThemeRestore ( )
    {
        // Arrange
        using var sut = CreateSut ( ) ;

        sut.Initialize ( ) ;

        // Use reflection to invoke private method OnDisplaySettingsChanged
        var method = typeof ( ThemeRestoreOnResume ).GetMethod ( "OnDisplaySettingsChanged" ,
                                                                 BindingFlags.Instance | BindingFlags.NonPublic ) ;
        method.Should ( ).NotBeNull ( ) ;

        // Act
        method.Invoke ( sut ,
                        [null , EventArgs.Empty] ) ;

        // Allow background task to run
        await Task.Delay ( 20 ) ;

        // Assert
        await _themeRestore.Received ( ).ApplyWithDelayAsync ( Arg.Any < CancellationToken > ( ) ) ;
    }

    [ Fact ]
    public async Task UserPreferenceChanged_ShouldTriggerOnlyForRelevantCategories ( )
    {
        // Arrange
        using var sut = CreateSut ( ) ;

        sut.Initialize ( ) ;

        var method = typeof ( ThemeRestoreOnResume ).GetMethod ( "OnUserPreferenceChanged" ,
                                                                 BindingFlags.Instance | BindingFlags.NonPublic ) ;
        method.Should ( ).NotBeNull ( ) ;

        // Act - relevant categories
        method.Invoke ( sut ,
                        [null , new UserPreferenceChangedEventArgs ( UserPreferenceCategory.General )] ) ;
        method.Invoke ( sut ,
                        [null , new UserPreferenceChangedEventArgs ( UserPreferenceCategory.Color )] ) ;

        // Act - irrelevant category
        method.Invoke ( sut ,
                        [null , new UserPreferenceChangedEventArgs ( UserPreferenceCategory.Desktop )] ) ;

        // Allow background tasks to run
        await Task.Delay ( 50 ) ;

        // Assert - General and Color should have triggered two additional calls (total >= 2)
        await _themeRestore.Received ( 2 ).ApplyWithDelayAsync ( Arg.Any < CancellationToken > ( ) ) ;
    }

    [ Fact ]
    public async Task Dispose_ShouldDisposePowerEvents_And_Unsubscribe ( )
    {
        // Arrange
        var sut = new ThemeRestoreOnResume ( _logger ,
                                             _powerEvents ,
                                             _themeRestore ) ;
        sut.Initialize ( ) ;

        // Act - dispose sut
        sut.Dispose ( ) ;

        // Assert - powerEvents.Dispose should be called
        _powerEvents.Received ( 1 ).Dispose ( ) ;

        // Reset received counts
        _themeRestore.ClearReceivedCalls ( ) ;

        // Try to raise PowerModeChanged after dispose - should not trigger
        _powerEvents.PowerModeChanged += Raise.Event < PowerModeChangedEventHandler > ( this ,
                                                                                        new
                                                                                            PowerModeChangedEventArgs ( PowerModes
                                                                                                                           .Resume ) ) ;

        await Task.Delay ( 20 ) ;

        // Assert - no calls after dispose
        await _themeRestore.DidNotReceive ( ).ApplyWithDelayAsync ( Arg.Any < CancellationToken > ( ) ) ;
    }
}