namespace Idasen.SystemTray.Win11
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