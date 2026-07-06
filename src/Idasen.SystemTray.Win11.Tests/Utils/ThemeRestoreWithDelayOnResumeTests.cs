using FluentAssertions ;
using Idasen.SystemTray.Win11.Interfaces ;
using Idasen.SystemTray.Win11.TraySettings ;
using Idasen.SystemTray.Win11.Utils ;
using Idasen.TestLogger ;
using NSubstitute ;
using Serilog.Events ;
using Wpf.Ui.Appearance ;

namespace Idasen.SystemTray.Win11.Tests.Utils ;

public class ThemeRestoreWithDelayOnResumeTests : IDisposable
{
    private readonly InMemoryLogger           _logger          = new( ) ;
    private readonly ISettingsManager         _settingsManager = Substitute.For < ISettingsManager > ( ) ;
    private readonly IApplicationThemeManager _themeManager    = Substitute.For < IApplicationThemeManager > ( ) ;
    private          bool                     _disposed ;

    public ThemeRestoreWithDelayOnResumeTests ( )
    {
        // Default settings
        var settings = new Settings ( ) ;
        _settingsManager.CurrentSettings.Returns ( settings ) ;

        _themeManager.ApplyAsync ( Arg.Any < ApplicationTheme > ( ) ).Returns ( Task.CompletedTask ) ;
    }

    public void Dispose ( )
    {
        Dispose ( true ) ;

        GC.SuppressFinalize ( this ) ;
    }

    protected virtual void Dispose ( bool disposing )
    {
        if ( _disposed )
            return ;

        if ( disposing ) _logger.Dispose ( ) ;

        _disposed = true ;
    }

    [ Fact ]
    public async Task ApplyWithDelayAsync_WhenThemeNotSet_LogsWarningAndDoesNotApply ( )
    {
        // Arrange
        var settings = _settingsManager.CurrentSettings ;
        settings.AppearanceSettings.ThemeName = "   " ;

        var sut = new ThemeRestoreWithDelayOnResume ( _logger ,
                                                      _settingsManager ,
                                                      _themeManager ) ;

        // Act
        await sut.ApplyWithDelayAsync ( CancellationToken.None ) ;

        // Assert
        _logger.Contains ( "Theme is not set!" ,
                           LogEventLevel.Warning )
               .Should ( )
               .BeTrue ( ) ;

        await _themeManager.DidNotReceive ( ).ApplyAsync ( Arg.Any < ApplicationTheme > ( ) ) ;
    }

    [ Fact ]
    public async Task ApplyWithDelayAsync_ValidTheme_AppliesTheme ( )
    {
        // Arrange
        var settings = _settingsManager.CurrentSettings ;
        settings.AppearanceSettings.ThemeName = nameof ( ApplicationTheme.Dark ) ;

        // Make GetAppTheme return the expected theme so loop breaks after first try
        _themeManager.GetAppTheme ( ).Returns ( ApplicationTheme.Dark ) ;

        var sut = new ThemeRestoreWithDelayOnResume ( _logger ,
                                                      _settingsManager ,
                                                      _themeManager ) ;

        // Act
        await sut.ApplyWithDelayAsync ( CancellationToken.None ) ;

        // Assert
        await _themeManager.Received ( 1 ).ApplyAsync ( ApplicationTheme.Dark ) ;
    }

    [ Fact ]
    public async Task ApplyWithDelayAsync_WhenApplyThrows_LogsError ( )
    {
        // Arrange
        var settings = _settingsManager.CurrentSettings ;
        settings.AppearanceSettings.ThemeName = nameof ( ApplicationTheme.Light ) ;

        var ex = new InvalidOperationException ( "apply failed" ) ;

        _themeManager.ApplyAsync ( Arg.Any < ApplicationTheme > ( ) ).Returns ( _ => throw ex ) ;

        // Ensure GetAppTheme does not return the requested theme so warnings may also be logged
        _themeManager.GetAppTheme ( ).Returns ( ApplicationTheme.Dark ) ;

        var sut = new ThemeRestoreWithDelayOnResume ( _logger ,
                                                      _settingsManager ,
                                                      _themeManager ) ;

        // Act
        await sut.ApplyWithDelayAsync ( CancellationToken.None ) ;

        // Assert - should log the caught exception at least once with the specific message
        _logger.Contains ( "Failed to reapply theme after display/user preference change" ,
                           LogEventLevel.Error )
               .Should ( )
               .BeTrue ( ) ;
    }
}
