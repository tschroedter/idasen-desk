using System ;

namespace Idasen.BluetoothLE.Characteristics.Common
{
    public static class ExceptionExtensions
    {
        public static bool IsBluetoothDisabledException(this Exception e)
        {
            return e != null &&
                   ((uint)e.HResult == 0x8007048F ||
                    (uint)e.HResult == 0x800710DF ||
                    (uint)e.HResult == 0x8000FFFF);
        }
    }
}