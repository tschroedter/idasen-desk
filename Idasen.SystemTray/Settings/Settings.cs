using Idasen.SystemTray.Interfaces ;

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