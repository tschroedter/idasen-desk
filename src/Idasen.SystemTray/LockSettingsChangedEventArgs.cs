using System ;

namespace Idasen.SystemTray
{
    public class LockSettingsChangedEventArgs
        : EventArgs
    {
        public bool IsLocked { get ; }

        public LockSettingsChangedEventArgs ( bool IsLocked )
        {
            this.IsLocked = IsLocked ;
        }
    }
}