namespace Idasen.RESTAPI.Desk.Emulator.Dtos ;

public class HeightDto
{
    public uint Height { get ; set ; }

    public override string ToString ( )
    {
        return $"[Height: {Height}]" ;
    }
}