using Idasen.SystemTray.Win11.TraySettings ;
using NSubstitute ;

namespace Idasen.SystemTray.Win11.Tests.TraySettings ;

public class SettingsChangesTests
    : IDisposable
{
    private readonly SettingsChanges _settingsChanges = new( ) ;

    public void Dispose ( )
    {
        _settingsChanges.Dispose ( ) ;

        GC.SuppressFinalize ( this ) ;
    }

    [ Fact ]
    public void LockSettingsChanged_ShouldEmitValue_WhenTriggered ( )
    {
        // Arrange  
        var observer = Substitute.For < IObserver < bool > > ( ) ;
        _settingsChanges.LockSettingsChanged.Subscribe ( observer ) ;

        // Act  
        _settingsChanges.LockSettingsChanged
                        .OnNext ( true ) ;

        // Assert  
        observer.Received ( 1 )
                .OnNext ( true ) ;
    }

    [ Fact ]
    public void AdvancedSettingsChanged_ShouldEmitValue_WhenTriggered ( )
    {
        // Arrange  
        var observer = Substitute.For < IObserver < bool > > ( ) ;
        _settingsChanges.AdvancedSettingsChanged.Subscribe ( observer ) ;

        // Act  
        _settingsChanges.AdvancedSettingsChanged
                        .OnNext ( false ) ;

        // Assert  
        observer.Received ( 1 )
                .OnNext ( false ) ;
    }
}