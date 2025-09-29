using System.ComponentModel ;
using System.Globalization ;
using System.Windows.Data ;
using Wpf.Ui.Appearance ;

namespace Idasen.SystemTray.Win11.Helpers ;

public class EnumToBooleanConverter : IValueConverter
{
    public object Convert ( object ? value , Type targetType , object ? parameter , CultureInfo culture )
    {
        if ( parameter is not string enumString )
            throw new ArgumentException ( $"{nameof ( parameter )} must be an enum name" ,
                                          nameof ( parameter ) ) ;

        if ( value is null )
            throw new ArgumentNullException ( nameof ( value ) ,
                                              $"{nameof ( value )} must not be null" ) ;

        if ( ! Enum.IsDefined ( typeof ( ApplicationTheme ) ,
                                value ) )
            throw new
                InvalidEnumArgumentException ( $"{nameof ( value )} must be a valid {nameof ( ApplicationTheme )} enum value" ) ;

        if ( ! Enum.TryParse ( typeof ( ApplicationTheme ) ,
                               enumString ,
                               out var enumValue ) )
            throw new ArgumentException ( $"'{enumString}' is not a valid {nameof ( ApplicationTheme )} value" ,
                                          nameof ( parameter ) ) ;

        return enumValue.Equals ( value ) ;
    }

    public object ConvertBack ( object ? value , Type targetType , object ? parameter , CultureInfo culture )
    {
        if ( parameter is not string enumString )
            throw new ArgumentException ( $"{nameof ( parameter )} must be an enum name" ,
                                          nameof ( parameter ) ) ;

        if ( value is null )
            throw new ArgumentNullException ( nameof ( value ) ,
                                              $"{nameof ( value )} must not be null" ) ;

        if ( ! Enum.TryParse ( typeof ( ApplicationTheme ) ,
                               enumString ,
                               out var enumValue ) )
            throw new ArgumentException ( $"'{enumString}' is not a valid {nameof ( ApplicationTheme )} value" ,
                                          nameof ( parameter ) ) ;

        return enumValue ;
    }
}