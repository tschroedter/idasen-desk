using System.Drawing ;
using System.Windows.Interop ;
using System.Windows.Media ;
using System.Windows.Media.Imaging ;

namespace Idasen.SystemTray.Win11.Utils.Icons ;

public static class IconExtensions
{
    public static ImageSource ToImageSource ( this Icon icon )
    {
        return icon == null
                   ? throw new ArgumentNullException ( nameof ( icon ) )
                   : Imaging.CreateBitmapSourceFromHIcon ( icon.Handle ,
                                                           Int32Rect.Empty ,
                                                           BitmapSizeOptions.FromEmptyOptions ( ) ) ;
    }
}