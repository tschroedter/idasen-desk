using System.Diagnostics.CodeAnalysis ;
using System.Windows.Controls ;
using System.Windows.Media ;
using MenuItem = Wpf.Ui.Controls.MenuItem ;

namespace Idasen.SystemTray.Win11.Utils ;

[ ExcludeFromCodeCoverage ]
public class CustomSeparatorMenuItem : MenuItem
{
    public CustomSeparatorMenuItem ( )
    {
        IsEnabled = false ;
        Template  = CreateSeparatorTemplate ( ) ;
    }

    private static ControlTemplate CreateSeparatorTemplate ( )
    {
        var borderFactory = new FrameworkElementFactory ( typeof ( Border ) ) ;
        borderFactory.SetValue ( HeightProperty ,
                                 1.0 ) ;
        borderFactory.SetValue ( MarginProperty ,
                                 new Thickness ( 4 ,
                                                 6 ,
                                                 4 ,
                                                 6 ) ) ;
        borderFactory.SetValue ( Border.BackgroundProperty ,
                                 Brushes.Gray ) ;

        var template = new ControlTemplate ( typeof ( MenuItem ) )
        {
            VisualTree = borderFactory
        } ;

        return template ;
    }
}