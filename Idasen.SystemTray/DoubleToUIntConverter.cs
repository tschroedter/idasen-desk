using System ;

namespace Idasen.SystemTray
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
            catch ( Exception _ )
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