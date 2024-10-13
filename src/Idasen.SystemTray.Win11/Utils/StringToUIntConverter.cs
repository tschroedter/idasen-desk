using Idasen.SystemTray.Win11.Interfaces ;

namespace Idasen.SystemTray.Win11.Utils ;

public class StringToUIntConverter
    : IStringToUIntConverter
{
    public ulong ConvertStringToUlongOrDefault ( string text ,
                                       ulong  defaultValue )
    {
        var isValid = ulong.TryParse ( text , out var value ) ;

        return isValid
                   ? value
                   : defaultValue ;
    }
}