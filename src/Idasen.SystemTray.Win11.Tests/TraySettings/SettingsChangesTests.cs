using FluentAssertions ;
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

    [ Fact ]
    public void Dispose_CompletesAndDisposesSubjects ( )
    {
        // Arrange
        var advancedCompleted = false ;
        var lockCompleted     = false ;
        _settingsChanges.AdvancedSettingsChanged.Subscribe ( _ => { } ,
                                                             ( ) => advancedCompleted = true ) ;
        _settingsChanges.LockSettingsChanged.Subscribe ( _ => { } ,
                                                         ( ) => lockCompleted = true ) ;

        // Act
        _settingsChanges.Dispose ( ) ;

        // Assert
        advancedCompleted.Should ( ).BeTrue ( ) ;
        lockCompleted.Should ( ).BeTrue ( ) ;
    }

    [ Fact ]
    public void Dispose_CanBeCalledMultipleTimes_Safely ( )
    {
        // Act
        _settingsChanges.Dispose ( ) ;
        var exception = Record.Exception ( ( ) => _settingsChanges.Dispose ( ) ) ;

        // Assert
        exception.Should ( ).BeNull ( ) ;
    }

    [ Fact ]
    public void AfterDispose_SubjectsAreCompletedAndDoNotEmitValues ( )
    {
        // Arrange
        var advancedReceived = false ;
        var lockReceived     = false ;
        _settingsChanges.AdvancedSettingsChanged.Subscribe ( _ => advancedReceived = true ) ;
        _settingsChanges.LockSettingsChanged.Subscribe ( _ => lockReceived         = true ) ;
        _settingsChanges.Dispose ( ) ;

        // Act & Assert
        var advancedException = Record.Exception ( ( ) => _settingsChanges.AdvancedSettingsChanged.OnNext ( true ) ) ;
        var lockException     = Record.Exception ( ( ) => _settingsChanges.LockSettingsChanged.OnNext ( true ) ) ;

        advancedReceived.Should ( ).BeFalse ( ) ;
        lockReceived.Should ( ).BeFalse ( ) ;
        advancedException.Should ( ).BeOfType < ObjectDisposedException > ( ) ;
        lockException.Should ( ).BeOfType < ObjectDisposedException > ( ) ;
    }
}