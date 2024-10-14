namespace Idasen.SystemTray.Win11.Utils ;

public record NotificationParameters (
    string     Title ,
    string     Text ,
    Visibility VisibilityBulbGreen  = Visibility.Hidden ,
    Visibility VisibilityBulbYellow = Visibility.Hidden ,
    Visibility VisibilityBulbRed    = Visibility.Hidden )
{
    public override string ToString ( )
    {
        return $"{nameof ( Title )} = {Title}, "                               +
               $"{nameof ( Text )} = {Text}, "                                 +
               $"{nameof ( VisibilityBulbGreen )} = {VisibilityBulbGreen}, "   +
               $"{nameof ( VisibilityBulbYellow )} = {VisibilityBulbYellow}, " +
               $"{nameof ( VisibilityBulbRed )} = {VisibilityBulbRed}" ;
    }
}