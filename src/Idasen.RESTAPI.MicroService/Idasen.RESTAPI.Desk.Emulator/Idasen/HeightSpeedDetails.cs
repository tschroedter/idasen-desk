namespace Idasen.RESTAPI.Desk.Emulator.Idasen ;

public class HeightSpeedDetails
{
    public HeightSpeedDetails ( DateTimeOffset timestamp ,
                                uint           height ,
                                int            speed )
    {
        Timestamp = timestamp ;
        Height    = height ;
        Speed     = speed ;
    }

    public DateTimeOffset Timestamp { get ; }
    public uint           Height    { get ; }
    public int            Speed     { get ; }

    public override string ToString ( )
    {
        return "["                            +
               $"Timestamp = {Timestamp:O}, " +
               $"Height = {Height}, "         +
               $"Speed = {Speed}"             +
               "]" ;
    }
}