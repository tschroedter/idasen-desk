using FluentAssertions ;
using Idasen.SystemTray.Win11.Utils ;
using Wpf.Ui.Controls ;

namespace Idasen.SystemTray.Win11.Tests.Utils ;

public class NotificationParametersTests
{
    private const string TestTitle = "Test Title" ;
    private const string TestText  = "Test Text" ;

    [ Fact ]
    public void ToString_ShouldReturnCorrectStringRepresentation ( )
    {
        // Arrange  
        var parameters = new NotificationParameters ( TestTitle ,
                                                      TestText ,
                                                      InfoBarSeverity.Warning ) ;

        // Act  
        var result = parameters.ToString ( ) ;

        // Assert  
        result.Should ( ).Be ( $"Title = {TestTitle}, Text = {TestText}, Severity = Warning" ) ;
    }

    [ Fact ]
    public void Constructor_ShouldSetPropertiesCorrectly ( )
    {
        // Arrange  

        // Act  
        var parameters = new NotificationParameters ( TestTitle ,
                                                      TestText ,
                                                      InfoBarSeverity.Warning ) ;

        // Assert  
        parameters.Title.Should ( ).Be ( TestTitle ) ;
        parameters.Text.Should ( ).Be ( TestText ) ;
        parameters.Severity.Should ( ).Be ( InfoBarSeverity.Warning ) ;
    }
}