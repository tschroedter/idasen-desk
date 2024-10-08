using System.Drawing ;
using System.Windows.Interop ;
using System.Windows.Media ;
using System.Windows.Media.Imaging ;

public static class IconExtensions
{
    public static ImageSource ToImageSource(this Icon icon)
    {
        if (icon == null)
            throw new ArgumentNullException(nameof(icon));

        return Imaging.CreateBitmapSourceFromHIcon(
                                                   icon.Handle,
                                                   Int32Rect.Empty,
                                                   BitmapSizeOptions.FromEmptyOptions());
    }
}