using System.Diagnostics.CodeAnalysis ;
using System.Drawing ;
using System.Windows.Interop ;
using System.Windows.Media ;
using System.Windows.Media.Imaging ;

namespace Idasen.SystemTray.Win11.Utils.Icons ;

[ ExcludeFromCodeCoverage ]
public static class IconExtensions
{
    /// <summary>
    ///     Converts a <see cref="Icon" /> to a WPF <see cref="ImageSource" />.
    /// </summary>
    /// <param name="icon">The icon to convert.</param>
    /// <returns>An <see cref="ImageSource" /> representation of the icon.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="icon" /> is null.</exception>
    public static ImageSource ToImageSource ( this Icon icon )
    {
        if ( icon == null )
            throw new ArgumentNullException ( nameof ( icon ) ) ;

        var imageSource = Imaging.CreateBitmapSourceFromHIcon ( icon.Handle ,
                                                                Int32Rect.Empty ,
                                                                BitmapSizeOptions.FromEmptyOptions ( ) ) ;

        imageSource.Freeze ( ) ; // Make the image thread-safe and more efficient

        return imageSource ;
    }
}