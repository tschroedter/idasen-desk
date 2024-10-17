using System.Globalization ;
using System.Windows.Data ;
using Wpf.Ui.Appearance ;

namespace Idasen.SystemTray.Win11.Helpers ;

internal class EnumToBooleanConverter : IValueConverter
{
    public object Convert ( object ?    value ,
                            Type        targetType ,
                            object ?    parameter ,
                            CultureInfo culture )
    {
        if ( parameter is not string enumString )
        {
            throw new ArgumentException ( $"{nameof ( parameter )} must be an enum name" ) ;
        }

        if ( value is null )
        {
            throw new ArgumentException ( $"{nameof ( value )} must not be null" ) ;
        }

        if ( ! Enum.IsDefined ( typeof ( ApplicationTheme ) , value ) )
        {
            throw new ArgumentException ( $"{nameof ( value )} must be an enum" ) ;
        }

        var enumValue = Enum.Parse ( typeof ( ApplicationTheme ) , enumString ) ;

        return enumValue.Equals ( value ) ;
    }

    public object ConvertBack ( object ?    value ,
                                Type        targetType ,
                                object ?    parameter ,
                                CultureInfo culture )
    {
        if ( parameter is not string enumString )
        {
            throw new ArgumentException ( $"{nameof ( parameter )} must be an enum name" ) ;
        }

        if ( value is null )
        {
            throw new ArgumentException ( $"{nameof ( value )} must not be null" ) ;
        }

        return Enum.Parse ( typeof ( ApplicationTheme ) , enumString ) ;
    }
}