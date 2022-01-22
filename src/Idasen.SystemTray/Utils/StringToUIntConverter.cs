using System ;
using Idasen.SystemTray.Interfaces ;

namespace Idasen.SystemTray.Utils
{
    public class StringToUIntConverter
        : IStringToUIntConverter
    {
        public ulong ConvertToULong(string text,
                                   ulong  defaultValue)
        {
            return !TryConvertToULong(text,
                                     out var uintValue)
                       ? defaultValue
                       : uintValue;
        }

        public bool TryConvertToULong(string   text,
                                     out ulong uintValue)
        {
            try
            {
                uintValue = Convert.ToUInt64(text);

                return true;
            }
            catch (Exception)
            {
                uintValue = 0;

                return false;
            }
        }
    }
}