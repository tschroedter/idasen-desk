using FluentAssertions ;
using Idasen.SystemTray.Win11.Utils ;
using Idasen.TestLogger ;
using Wpf.Ui.Controls ;

namespace Idasen.SystemTray.Win11.Tests.Utils ;

public class StatusBarManagerTests
{
    [ Fact ]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException ( )
    {
        // Act
        var act = ( ) => new StatusBarManager ( null! ) ;

        // Assert
        act.Should ( ).Throw < ArgumentNullException > ( )
           .WithParameterName ( "logger" ) ;
    }

    [ Fact ]
    public void StatusBarInfoChanged_ReturnsObservable ( )
    {
        // Arrange
        using var logger = new InMemoryLogger ( ) ;
        var       sut    = new StatusBarManager ( logger ) ;

        // Act
        var observable = sut.StatusBarInfoChanged ;

        // Assert
        observable.Should ( ).NotBeNull ( ) ;
    }

    [ Fact ]
    public void UpdateStatus_WithNullInfo_ThrowsArgumentNullException ( )
    {
        // Arrange
        using var logger = new InMemoryLogger ( ) ;
        var       sut    = new StatusBarManager ( logger ) ;

        // Act
        var act = ( ) => sut.UpdateStatus ( null! ) ;

        // Assert
        act.Should ( ).Throw < ArgumentNullException > ( )
           .WithParameterName ( "info" ) ;
    }

    [ Fact ]
    public void UpdateStatus_PublishesStatusBarInfo ( )
    {
        // Arrange
        using var logger = new InMemoryLogger ( ) ;
        var       sut    = new StatusBarManager ( logger ) ;

        StatusBarInfo ? receivedInfo = null ;
        sut.StatusBarInfoChanged.Subscribe ( info => receivedInfo = info ) ;

        var expectedInfo = new StatusBarInfo ( "Test" ,
                                               1200 ,
                                               "Test Message" ,
                                               InfoBarSeverity.Success ) ;

        // Act
        sut.UpdateStatus ( expectedInfo ) ;

        // Assert
        receivedInfo.Should ( ).NotBeNull ( ) ;
        receivedInfo!.Title.Should ( ).Be ( "Test" ) ;
        receivedInfo.Height.Should ( ).Be ( 1200u ) ;
        receivedInfo.Message.Should ( ).Be ( "Test Message" ) ;
        receivedInfo.Severity.Should ( ).Be ( InfoBarSeverity.Success ) ;
    }

    [ Fact ]
    public void UpdateDeskHeight_PublishesStatusBarInfoWithHeightInCm ( )
    {
        // Arrange
        using var logger = new InMemoryLogger ( ) ;
        var       sut    = new StatusBarManager ( logger ) ;

        StatusBarInfo ? receivedInfo = null ;
        sut.StatusBarInfoChanged.Subscribe ( info => receivedInfo = info ) ;

        // Act
        sut.UpdateDeskHeight ( 12000 ) ; // 120 cm

        // Assert
        receivedInfo.Should ( ).NotBeNull ( ) ;
        receivedInfo!.Message.Should ( ).Be ( "Height: 120 cm" ) ;
    }

    [ Fact ]
    public void UpdateDeskHeight_ConvertsMillimetersToCentimeters ( )
    {
        // Arrange
        using var logger = new InMemoryLogger ( ) ;
        var       sut    = new StatusBarManager ( logger ) ;

        StatusBarInfo ? receivedInfo = null ;
        sut.StatusBarInfoChanged.Subscribe ( info => receivedInfo = info ) ;

        // Act
        sut.UpdateDeskHeight ( 7500 ) ; // 75 cm

        // Assert
        receivedInfo.Should ( ).NotBeNull ( ) ;
        receivedInfo!.Message.Should ( ).Be ( "Height: 75 cm" ) ;
    }

    [ Fact ]
    public void Dispose_CompletesObservable ( )
    {
        // Arrange
        using var logger = new InMemoryLogger ( ) ;
        var       sut    = new StatusBarManager ( logger ) ;

        var completed = false ;
        sut.StatusBarInfoChanged.Subscribe (
                                            _ => { } ,
                                            ( ) => completed = true
                                           ) ;

        // Act
        sut.Dispose ( ) ;

        // Assert
        completed.Should ( ).BeTrue ( ) ;
    }

    [ Fact ]
    public void Dispose_CalledMultipleTimes_DoesNotThrow ( )
    {
        // Arrange
        using var logger = new InMemoryLogger ( ) ;
        var       sut    = new StatusBarManager ( logger ) ;

        // Act
        var act = ( ) =>
                  {
#pragma warning disable S3966
                      sut.Dispose ( ) ;
                      sut.Dispose ( ) ;
#pragma warning restore S3966
                  } ;

        // Assert
        act.Should ( ).NotThrow ( ) ;
    }

    [ Fact ]
    public void StatusBarInfoChanged_MultipleSubscribers_AllReceiveUpdates ( )
    {
        // Arrange
        using var logger = new InMemoryLogger ( ) ;
        var       sut    = new StatusBarManager ( logger ) ;

        StatusBarInfo ? receivedInfo1 = null ;
        StatusBarInfo ? receivedInfo2 = null ;

        sut.StatusBarInfoChanged.Subscribe ( info => receivedInfo1 = info ) ;
        sut.StatusBarInfoChanged.Subscribe ( info => receivedInfo2 = info ) ;

        var expectedInfo = new StatusBarInfo ( "Test" ,
                                               1000 ,
                                               "Test Message" ,
                                               InfoBarSeverity.Warning ) ;

        // Act
        sut.UpdateStatus ( expectedInfo ) ;

        // Assert
        receivedInfo1.Should ( ).NotBeNull ( ) ;
        receivedInfo1!.Title.Should ( ).Be ( "Test" ) ;
        receivedInfo1.Message.Should ( ).Be ( "Test Message" ) ;

        receivedInfo2.Should ( ).NotBeNull ( ) ;
        receivedInfo2!.Title.Should ( ).Be ( "Test" ) ;
        receivedInfo2.Message.Should ( ).Be ( "Test Message" ) ;
    }

    [ Fact ]
    public void UpdateStatus_AfterDispose_DoesNotPublish ( )
    {
        // Arrange
        using var logger = new InMemoryLogger ( ) ;
        var       sut    = new StatusBarManager ( logger ) ;

        sut.StatusBarInfoChanged.Subscribe ( info => _ = info ) ;

        sut.Dispose ( ) ;

        var testInfo = new StatusBarInfo ( "Test" ,
                                           1000 ,
                                           "Test Message" ,
                                           InfoBarSeverity.Error ) ;

        // Act (should not throw but won't publish to disposed subject)
        var act = ( ) => sut.UpdateStatus ( testInfo ) ;

        // Assert
        act.Should ( ).Throw < ObjectDisposedException > ( ) ;
    }

    [ Fact ]
    public void UpdateDeskHeight_WithDifferentHeights_PublishesCorrectValues ( )
    {
        // Arrange
        using var logger = new InMemoryLogger ( ) ;
        var       sut    = new StatusBarManager ( logger ) ;

        var receivedInfos = new List < StatusBarInfo > ( ) ;
        sut.StatusBarInfoChanged.Subscribe ( info => receivedInfos.Add ( info ) ) ;

        // Act
        sut.UpdateDeskHeight ( 6500 ) ;  // 65 cm
        sut.UpdateDeskHeight ( 12000 ) ; // 120 cm
        sut.UpdateDeskHeight ( 8500 ) ;  // 85 cm

        // Assert
        receivedInfos.Should ( ).HaveCount ( 3 ) ;
        receivedInfos [ 0 ].Message.Should ( ).Be ( "Height: 65 cm" ) ;
        receivedInfos [ 0 ].Height.Should ( ).Be ( 6500u ) ;
        receivedInfos [ 1 ].Message.Should ( ).Be ( "Height: 120 cm" ) ;
        receivedInfos [ 1 ].Height.Should ( ).Be ( 12000u ) ;
        receivedInfos [ 2 ].Message.Should ( ).Be ( "Height: 85 cm" ) ;
        receivedInfos [ 2 ].Height.Should ( ).Be ( 8500u ) ;
    }
}
