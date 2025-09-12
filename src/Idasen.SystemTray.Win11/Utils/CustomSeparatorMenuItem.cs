using System.Diagnostics.CodeAnalysis ;
using System.Windows.Controls;
using System.Windows.Media;

namespace Idasen.SystemTray.Win11.Utils ;

[ExcludeFromCodeCoverage]
public class CustomSeparatorMenuItem : Wpf.Ui.Controls.MenuItem
{
    public CustomSeparatorMenuItem()
    {
        IsEnabled = false;
        Template  = CreateSeparatorTemplate();
    }

    private static ControlTemplate CreateSeparatorTemplate()
    {
        var borderFactory = new FrameworkElementFactory(typeof(Border));
        borderFactory.SetValue(Border.HeightProperty, 1.0);
        borderFactory.SetValue(Border.MarginProperty, new Thickness(4, 6, 4, 6));
        borderFactory.SetValue(Border.BackgroundProperty, Brushes.Gray);

        var template = new ControlTemplate(typeof(Wpf.Ui.Controls.MenuItem))
        {
            VisualTree = borderFactory
        } ;

        return template;
    }
}