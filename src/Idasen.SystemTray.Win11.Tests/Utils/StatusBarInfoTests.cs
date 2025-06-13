using FluentAssertions ;
using Idasen.SystemTray.Win11.Utils ;
using Wpf.Ui.Controls ;

namespace Idasen.SystemTray.Win11.Tests.Utils ;

public class StatusBarInfoTests
{
    private const string TestTitle   = "Test Title" ;
    private const string TestMessage = "Test Message" ;
    private const uint   Height      = 100u ;

    [ Fact ]
    public void ToString_ShouldReturnCorrectStringRepresentation ( )
    {
        // Arrange  
        var statusBarInfo = new StatusBarInfo ( TestTitle ,
                                                Height ,
                                                TestMessage ,
                                                InfoBarSeverity.Warning ) ;

        var expected = $"Title = {TestTitle}, "     +
                       $"Height = {Height}, "       +
                       $"Message = {TestMessage}, " +
                       $"Severity = {InfoBarSeverity.Warning}" ;

        // Act  
        var result = statusBarInfo.ToString ( ) ;

        // Assert  
        result.Should ( ).Be ( expected ) ;
    }

    [ Fact ]
    public void Constructor_ShouldSetPropertiesCorrectly ( )
    {
        // Arrange  

        // Act  
        var statusBarInfo = new StatusBarInfo ( TestTitle ,
                                                Height ,
                                                TestMessage ,
                                                InfoBarSeverity.Warning ) ;

        // Assert  
        statusBarInfo.Title.Should ( ).Be ( TestTitle ) ;
        statusBarInfo.Height.Should ( ).Be ( Height ) ;
        statusBarInfo.Message.Should ( ).Be ( TestMessage ) ;
        statusBarInfo.Severity.Should ( ).Be ( InfoBarSeverity.Warning ) ;
    }
}