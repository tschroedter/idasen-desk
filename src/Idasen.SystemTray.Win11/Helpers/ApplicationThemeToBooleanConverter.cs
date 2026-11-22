using System.ComponentModel ;
using System.Globalization ;
using System.Windows.Data ;
using Wpf.Ui.Appearance ;

namespace Idasen.SystemTray.Win11.Helpers ;

public class ApplicationThemeToBooleanConverter : IValueConverter
{
    public object Convert ( object ? value ,
                            Type targetType ,
                            object ? parameter ,
                            CultureInfo culture )
    {
        var theme = ParseTheme ( parameter ) ;

        if ( value is null )
            throw new ArgumentNullException ( nameof ( value ) ,
                                              $"{nameof ( value )} must not be null" ) ;

        if ( value is not ApplicationTheme currentTheme )
            throw new ArgumentException ( $"{nameof ( value )} must be an {nameof ( ApplicationTheme )} value" ,
                                          nameof ( value ) ) ;

        if ( ! Enum.IsDefined ( currentTheme ) )
            throw new
                InvalidEnumArgumentException ( $"{nameof ( value )} must be a valid {nameof ( ApplicationTheme )} enum value" ) ;

        return currentTheme == theme ;
    }

    public object ConvertBack ( object ? value ,
                                Type targetType ,
                                object ? parameter ,
                                CultureInfo culture )
    {
        var theme = ParseTheme ( parameter ) ;

        if ( value is null )
            throw new ArgumentNullException ( nameof ( value ) ,
                                              $"{nameof ( value )} must not be null" ) ;

        if ( value is not bool isChecked )
            throw new ArgumentException ( $"{nameof ( value )} must be a Boolean" ,
                                          nameof ( value ) ) ;

        return isChecked
                   ? theme
                   : Binding.DoNothing ;
    }

    private static ApplicationTheme ParseTheme ( object ? parameter )
    {
        if ( parameter is not string enumString )
            throw new ArgumentException ( $"{nameof ( parameter )} must be an enum name" ,
                                          nameof ( parameter ) ) ;

        if ( ! Enum.TryParse ( enumString ,
                               out ApplicationTheme theme ) ||
             ! Enum.IsDefined ( theme ) )
            throw new ArgumentException ( $"'{enumString}' is not a valid {nameof ( ApplicationTheme )} value" ,
                                          nameof ( parameter ) ) ;

        return theme ;
    }
}
