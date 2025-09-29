using Idasen.SystemTray.Win11.Interfaces ;

namespace Idasen.SystemTray.Win11.Utils.Converters ;

public class DoubleToUIntConverter
    : IDoubleToUIntConverter
{
    public uint ConvertToUInt ( double value ,
                                uint   defaultValue )
    {
        return ! TryConvertToUInt ( value ,
                                    out var uintValue )
                   ? defaultValue
                   : uintValue ;
    }

    public static bool TryConvertToUInt ( double   value ,
                                          out uint uintValue )
    {
        try
        {
            uintValue = Convert.ToUInt32 ( value ) ;

            return true ;
        }
        catch ( Exception )
        {
            uintValue = 0 ;

            return false ;
        }
    }
}