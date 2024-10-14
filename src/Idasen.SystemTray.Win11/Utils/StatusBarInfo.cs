using Wpf.Ui.Controls ;

namespace Idasen.SystemTray.Win11.Utils ;

public record StatusBarInfo ( string Title , uint Height , string Message , InfoBarSeverity Severity , Visibility Visibility )
{
    public override string ToString ( )
    {
        return $"{nameof ( Title )} = {Title}, "       +
               $"{nameof ( Height )} = {Height}, "     +
               $"{nameof ( Message )} = {Message}, "   +
               $"{nameof ( Severity )} = {Severity}, " +
               $"{nameof ( Visibility )} = {Visibility}" ;
    }
}