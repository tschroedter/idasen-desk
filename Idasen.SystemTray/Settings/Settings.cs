using Idasen.SystemTray.Interfaces ;
using Idasen.SystemTray.Utils ;

namespace Idasen.SystemTray.Settings
{
    public class Settings : ISettings
    {
        public uint StandingHeightInCm { get ; set ; } = Constants.DefaultHeightStandingInCm ;
        public uint SeatingHeightInCm  { get ; set ; } = Constants.DefaultHeightSeatingInCm ;

        public override string ToString ( )
        {
            return $"StandingHeightInCm = {StandingHeightInCm}, " +
                   $"SeatingHeightInCm = {SeatingHeightInCm}" ;
        }
    }
}