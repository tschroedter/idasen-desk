using System ;
using Idasen.SystemTray.Interfaces ;

namespace Idasen.SystemTray.Converters
{
    public class DoubleToUIntConverter
        : IDoubleToUIntConverter
    {
        public bool TryConvertToUInt ( double   value ,
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

        public uint TryConvertToUInt(double value,
                                     uint   defaultValue)
        {
            return ! TryConvertToUInt ( value ,
                                        out var uintValue )
                       ? defaultValue
                       : uintValue ;
        }
    }
}