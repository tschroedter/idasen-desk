using FluentAssertions ;
using Idasen.SystemTray.Win11.Utils ;
using Serilog ;
using NSubstitute ;

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
        var logger = Substitute.For<ILogger>();

        return new VersionProvider(logger);
    }
}