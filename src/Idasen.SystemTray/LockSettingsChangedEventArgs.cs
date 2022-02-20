using System ;

namespace Idasen.SystemTray
{
    public class LockSettingsChangedEventArgs
        : EventArgs
    {
        public bool IsLocked { get ; }

        public LockSettingsChangedEventArgs ( bool isLocked )
        {
            this.IsLocked = isLocked ;
        }
    }
}