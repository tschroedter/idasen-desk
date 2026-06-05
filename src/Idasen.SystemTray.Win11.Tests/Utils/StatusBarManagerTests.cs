using FluentAssertions ;
using Idasen.SystemTray.Win11.Utils ;
using NSubstitute ;
using Serilog ;
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
        var logger = Substitute.For < ILogger > ( ) ;
        var sut    = new StatusBarManager ( logger ) ;

        // Act
        var observable = sut.StatusBarInfoChanged ;

        // Assert
        observable.Should ( ).NotBeNull ( ) ;
    }

    [ Fact ]
    public void UpdateStatus_WithNullInfo_ThrowsArgumentNullException ( )
    {
        // Arrange
        var logger = Substitute.For < ILogger > ( ) ;
        var sut    = new StatusBarManager ( logger ) ;

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
        var logger = Substitute.For < ILogger > ( ) ;
        var sut    = new StatusBarManager ( logger ) ;

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
        var logger = Substitute.For < ILogger > ( ) ;
        var sut    = new StatusBarManager ( logger ) ;

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
        var logger = Substitute.For < ILogger > ( ) ;
        var sut    = new StatusBarManager ( logger ) ;

        StatusBarInfo ? receivedInfo = null ;
        sut.StatusBarInfoChanged.Subscribe ( info => receivedInfo = info ) ;

        // Act
        sut.UpdateDeskHeight ( 7500 ) ; // 75 cm

        // Assert
        receivedInfo.Should ( ).NotBeNull ( ) ;
        receivedInfo!.Message.Should ( ).Be ( "Height: 75 cm" ) ;
    }
}
