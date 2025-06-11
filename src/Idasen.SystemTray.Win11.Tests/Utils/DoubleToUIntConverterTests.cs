using FluentAssertions ;
using Idasen.SystemTray.Win11.Utils.Converters ;

namespace Idasen.SystemTray.Win11.Tests.Utils ;

public class DoubleToUIntConverterTests
{
    [ Theory ]
    [ InlineData ( 123.45 ,
                   123 ) ]
    [ InlineData ( 0.0 ,
                   0 ) ]
    [ InlineData ( - 1.0 ,
                   0 ) ]
    public void ConvertToUInt_ShouldReturnExpectedValue ( double input , uint expected )
    {
        // Act  
        var result = CreateSut ( ).ConvertToUInt ( input ,
                                                   0 ) ;

        // Assert  
        result.Should ( ).Be ( expected ) ;
    }

    [ Fact ]
    public void ConvertToUInt_ShouldReturnDefaultValue_WhenConversionFails ( )
    {
        // Arrange  
        var input        = double.MaxValue ;
        var defaultValue = 42u ;

        // Act  
        var result = CreateSut ( ).ConvertToUInt ( input ,
                                                   defaultValue ) ;

        // Assert  
        result.Should ( ).Be ( defaultValue ) ;
    }

    [ Theory ]
    [ InlineData ( 123.45 ,
                   true ,
                   123 ) ]
    [ InlineData ( - 1.0 ,
                   false ,
                   0 ) ]
    [ InlineData ( double.MaxValue ,
                   false ,
                   0 ) ]
    public void TryConvertToUInt_ShouldReturnExpectedResult ( double input , bool expectedSuccess , uint expectedValue )
    {
        // Act  
        var success = CreateSut ( ).TryConvertToUInt ( input ,
                                                       out var result ) ;

        // Assert  
        success.Should ( ).Be ( expectedSuccess ) ;
        result.Should ( ).Be ( expectedValue ) ;
    }

    private static DoubleToUIntConverter CreateSut ( )
    {
        return new DoubleToUIntConverter ( ) ;
    }
}