using FluentAssertions ;
using Idasen.SystemTray.Win11.TraySettings ;
using Idasen.SystemTray.Win11.Utils ;

namespace Idasen.SystemTray.Win11.Tests.TraySettings ;

public class CommonApplicationDataTests
{
    [ Fact ]
    public void ToFullPath_ShouldCombineFolderNameAndFileName ( )
    {
        // Arrange  
        var fileName = "test.txt" ;
        var expectedPath = Path.Combine ( Environment.GetFolderPath ( Environment.SpecialFolder.CommonApplicationData ) ,
                                          Constants.ApplicationName ,
                                          fileName ) ;

        // Act  
        var result = CreateSut ( ).ToFullPath ( fileName ) ;

        // Assert  
        result.Should ( )
              .Be ( expectedPath ) ;
    }

    [ Fact ]
    public void FolderName_ShouldReturnCorrectPath ( )
    {
        // Arrange  
        var expectedFolder = Path.Combine ( Environment.GetFolderPath ( Environment.SpecialFolder.CommonApplicationData ) ,
                                            Constants.ApplicationName ) ;

        // Act  
        var result = CreateSut ( ).FolderName ( ) ;

        // Assert  
        result.Should ( )
              .Be ( expectedFolder ) ;
    }

    private static CommonApplicationData CreateSut ( )
    {
        return new CommonApplicationData ( ) ;
    }
}