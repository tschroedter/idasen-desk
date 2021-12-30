namespace Idasen.RESTAPI.Dtos
{
    public class SettingsDto
    {
        private const string DefaultId             = "Unknown" ;
        private const uint   DefaultHeightSeating  = 6000u ;
        private const uint   DefaultHeightStanding = 12000u ;

        public string Id       { get ; set ; } = DefaultId ;
        public uint   Seating  { get ; set ; } = DefaultHeightSeating ;
        public uint   Standing { get ; set ; } = DefaultHeightStanding ;

        public override string ToString ( )
        {
            return $"[Id: {Id}, Seating: {Seating}, Standing: {Standing}]" ;
        }
    }
}