using Wpf.Ui.Controls ;

namespace Idasen.SystemTray.Win11.Utils ;

public record NotificationParameters ( string          Title ,
                                       string          Text ,
                                       InfoBarSeverity Severity )
{
    public override string ToString ( )
    {
        return $"{nameof ( Title )} = {Title}, " +
               $"{nameof ( Text )} = {Text}, "   +
               $"{nameof ( Severity )} = {Severity}" ;
    }
}