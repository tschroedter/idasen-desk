using System.Reflection ;
using FluentAssertions ;
using Idasen.SystemTray.Win11.Utils ;
using NSubstitute ;
using Serilog ;

namespace Idasen.SystemTray.Win11.Tests.Utils ;

public class VersionProviderTests
{
    [ Fact ]
    public void GetVersion_ShouldReturnDefaultVersion_WhenAssemblyVersionIsNull ( )
    {
        // Arrange  

        // Act  
        var result = CreateSut ( ).GetVersion ( ) ;

        // Assert  
        result.Should ( ).StartWith ( "V" ) ;
    }

    private static VersionProvider CreateSut()
    {
        return new VersionProvider();
    }
}