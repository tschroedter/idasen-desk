using FluentAssertions ;
using Idasen.SystemTray.Win11.Utils ;
using NSubstitute ;
using Wpf.Ui.Appearance ;

namespace Idasen.SystemTray.Win11.Tests.Utils ;

public class ThemeSwitcherTests
{
    [ Theory ]
    [ InlineData ( ApplicationTheme.Unknown ,
                   "light" ,
                   ApplicationTheme.Light ) ]
    [ InlineData ( ApplicationTheme.Unknown ,
                   "theme_light" ,
                   ApplicationTheme.Light ) ]
    [ InlineData ( ApplicationTheme.Unknown ,
                   "dark" ,
                   ApplicationTheme.Dark ) ]
    [ InlineData ( ApplicationTheme.Unknown ,
                   "theme_dark" ,
                   ApplicationTheme.Dark ) ]
    [ InlineData ( ApplicationTheme.Unknown ,
                   "highcontrast" ,
                   ApplicationTheme.HighContrast ) ]
    [ InlineData ( ApplicationTheme.Unknown ,
                   "high_contrast" ,
                   ApplicationTheme.HighContrast ) ]
    [ InlineData ( ApplicationTheme.Unknown ,
                   "theme_high_contrast" ,
                   ApplicationTheme.HighContrast ) ]
    [ InlineData ( ApplicationTheme.Light ,
                   "unknown" ,
                   ApplicationTheme.Unknown ) ]
    [ InlineData ( ApplicationTheme.Light ,
                   "other" ,
                   ApplicationTheme.Unknown ) ]
    public void ChangeTheme_AppliesCorrectTheme_WhenThemeIsDifferent ( ApplicationTheme currentTheme ,
                                                                       string           parameter ,
                                                                       ApplicationTheme expectedTheme )
    {
        // Arrange
        var themeManager = Substitute.For < IApplicationThemeManager > ( ) ;
        themeManager.GetAppTheme ( ).Returns ( currentTheme ) ;
        var switcher = new ThemeSwitcher ( themeManager ) ;

        // Act
        switcher.ChangeTheme ( parameter ) ;

        // Assert
        themeManager.Received ( 1 ).Apply ( expectedTheme ) ;
    }

    [ Theory ]
    [ InlineData ( "light" ,
                   ApplicationTheme.Light ) ]
    [ InlineData ( "dark" ,
                   ApplicationTheme.Dark ) ]
    [ InlineData ( "highcontrast" ,
                   ApplicationTheme.HighContrast ) ]
    [ InlineData ( "unknown" ,
                   ApplicationTheme.Unknown ) ]
    public void ChangeTheme_DoesNotApplyTheme_WhenThemeIsAlreadySet ( string parameter , ApplicationTheme currentTheme )
    {
        // Arrange
        var themeManager = Substitute.For < IApplicationThemeManager > ( ) ;
        themeManager.GetAppTheme ( ).Returns ( currentTheme ) ;
        var switcher = new ThemeSwitcher ( themeManager ) ;

        // Act
        switcher.ChangeTheme ( parameter ) ;

        // Assert
        themeManager.DidNotReceive ( ).Apply ( Arg.Any < ApplicationTheme > ( ) ) ;
    }

    [ Theory ]
    [ InlineData ( ApplicationTheme.Light ) ]
    [ InlineData ( ApplicationTheme.Dark ) ]
    [ InlineData ( ApplicationTheme.HighContrast ) ]
    [ InlineData ( ApplicationTheme.Unknown ) ]
    public void CurrentThemeName_ReturnsThemeName ( ApplicationTheme theme )
    {
        // Arrange
        var themeManager = Substitute.For < IApplicationThemeManager > ( ) ;
        themeManager.GetAppTheme ( ).Returns ( theme ) ;
        var switcher = new ThemeSwitcher ( themeManager ) ;

        // Act
        var result = switcher.CurrentThemeName ;

        // Assert
        result.Should ( ).Be ( theme.ToString ( ) ) ;
    }
}