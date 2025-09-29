using System.ComponentModel ;
using System.Globalization ;
using Idasen.SystemTray.Win11.Helpers ;
using Wpf.Ui.Appearance ;

namespace Idasen.SystemTray.Win11.Tests.Helpers ;

public class EnumToBooleanConverterTests
{
    [ Theory ]
    [ InlineData ( ApplicationTheme.Light ,
                   "Light" ,
                   true ) ]
    [ InlineData ( ApplicationTheme.Dark ,
                   "Light" ,
                   false ) ]
    [ InlineData ( ApplicationTheme.Dark ,
                   "Dark" ,
                   true ) ]
    [ InlineData ( ApplicationTheme.HighContrast ,
                   "Dark" ,
                   false ) ]
    public void Convert_ReturnsExpectedBoolean ( ApplicationTheme value , string parameter , bool expected )
    {
        var result = CreateSut ( ).Convert ( value ,
                                             typeof ( bool ) ,
                                             parameter ,
                                             CultureInfo.InvariantCulture ) ;
        Assert.Equal ( expected ,
                       result ) ;
    }

    [ Theory ]
    [ InlineData ( "Light" ,
                   ApplicationTheme.Light ) ]
    [ InlineData ( "Dark" ,
                   ApplicationTheme.Dark ) ]
    [ InlineData ( "HighContrast" ,
                   ApplicationTheme.HighContrast ) ]
    public void ConvertBack_ReturnsExpectedEnum ( string parameter , ApplicationTheme expected )
    {
        var result = CreateSut ( ).ConvertBack ( true ,
                                                 typeof ( ApplicationTheme ) ,
                                                 parameter ,
                                                 CultureInfo.InvariantCulture ) ;
        Assert.Equal ( expected ,
                       result ) ;
    }

    [ Fact ]
    public void Convert_ThrowsArgumentException_WhenParameterIsNotString ( )
    {
        Assert.Throws < ArgumentException > ( ( ) =>
                                                  CreateSut ( ).Convert ( ApplicationTheme.Light ,
                                                                          typeof ( bool ) ,
                                                                          123 ,
                                                                          CultureInfo.InvariantCulture ) ) ;
    }

    [ Fact ]
    public void Convert_ThrowsArgumentNullException_WhenValueIsNull ( )
    {
        Assert.Throws < ArgumentNullException > ( ( ) =>
                                                      CreateSut ( ).Convert ( null ,
                                                                              typeof ( bool ) ,
                                                                              "Light" ,
                                                                              CultureInfo.InvariantCulture ) ) ;
    }

    [ Fact ]
    public void Convert_ThrowsInvalidEnumArgumentException_WhenValueIsInvalidEnum ( )
    {
        Assert.Throws < InvalidEnumArgumentException > ( ( ) =>
                                                             CreateSut ( ).Convert ( ( ApplicationTheme )999 ,
                                                                      typeof ( bool ) ,
                                                                      "Light" ,
                                                                      CultureInfo.InvariantCulture ) ) ;
    }

    [ Fact ]
    public void Convert_ThrowsArgumentException_WhenParameterIsInvalidEnumName ( )
    {
        Assert.Throws < ArgumentException > ( ( ) =>
                                                  CreateSut ( ).Convert ( ApplicationTheme.Light ,
                                                                          typeof ( bool ) ,
                                                                          "InvalidEnum" ,
                                                                          CultureInfo.InvariantCulture ) ) ;
    }

    [ Fact ]
    public void ConvertBack_ThrowsArgumentException_WhenParameterIsNotString ( )
    {
        Assert.Throws < ArgumentException > ( ( ) =>
                                                  CreateSut ( ).ConvertBack ( true ,
                                                                              typeof ( ApplicationTheme ) ,
                                                                              123 ,
                                                                              CultureInfo.InvariantCulture ) ) ;
    }

    [ Fact ]
    public void ConvertBack_ThrowsArgumentNullException_WhenValueIsNull ( )
    {
        Assert.Throws < ArgumentNullException > ( ( ) =>
                                                      CreateSut ( ).ConvertBack ( null ,
                                                               typeof ( ApplicationTheme ) ,
                                                               "Light" ,
                                                               CultureInfo.InvariantCulture ) ) ;
    }

    [ Fact ]
    public void ConvertBack_ThrowsArgumentException_WhenParameterIsInvalidEnumName ( )
    {
        Assert.Throws < ArgumentException > ( ( ) =>
                                                  CreateSut ( ).ConvertBack ( true ,
                                                                              typeof ( ApplicationTheme ) ,
                                                                              "InvalidEnum" ,
                                                                              CultureInfo.InvariantCulture ) ) ;
    }

    private static EnumToBooleanConverter CreateSut ( )
    {
        return new EnumToBooleanConverter ( ) ;
    }
}