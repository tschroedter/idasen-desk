using System ;
using Idasen.SystemTray.Interfaces ;

namespace Idasen.SystemTray.Utils ;

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