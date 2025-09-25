using System.Diagnostics.CodeAnalysis ;
using System.Drawing ;
using System.Runtime.InteropServices ;
using System.Windows.Interop ;
using System.Windows.Media ;
using System.Windows.Media.Imaging ;
using Idasen.BluetoothLE.Core ;
using Idasen.SystemTray.Win11.Interfaces ;
using Wpf.Ui.Tray.Controls ;
using Color = System.Drawing.Color ;
using Pen = System.Drawing.Pen ;

namespace Idasen.SystemTray.Win11.Utils.Icons ;

[ ExcludeFromCodeCoverage ]
public class DynamicIconCreator : IDynamicIconCreator
{
    private const           int    IconHeight     = 16 ;
    private const           int    FontSize       = 8 ;
    private const           string FontFamily     = "Consolas" ;
    private static readonly Color  BrushDarkBlue  = ColorTranslator.FromHtml ( "#FF0048A3" ) ;
    private static readonly Color  BrushLightBlue = ColorTranslator.FromHtml ( "#FF0098F3" ) ;

    public void Update ( NotifyIcon taskbarIcon , int height )
    {
        Guard.ArgumentNotNull ( taskbarIcon ,
                                nameof ( taskbarIcon ) ) ;

        var image = CreateImageSource ( height ) ;

        PushIcons ( taskbarIcon ,
                    image ,
                    height ) ;
    }

    private ImageSource CreateImageSource ( int height )
    {
        var width = height >= 100
                        ? 24
                        : 16 ;

        using var bitmap = new Bitmap ( width ,
                                        IconHeight ) ;
        using var graphics = Graphics.FromImage ( bitmap ) ;
        using var pen      = new Pen ( BrushDarkBlue ) ;
        using var brush    = new SolidBrush ( BrushLightBlue ) ;
        using var font = new Font ( FontFamily ,
                                    FontSize ) ;

        // Draw top and bottom horizontal lines
        graphics.DrawLine ( pen ,
                            0 ,
                            0 ,
                            width ,
                            0 ) ;
        graphics.DrawLine ( pen ,
                            0 ,
                            IconHeight - 1 ,
                            width ,
                            IconHeight - 1 ) ;

        // Draw the height value string
        graphics.DrawString ( $"{height}" ,
                              font ,
                              brush ,
                              new PointF ( - 1 ,
                                           1 ) ) ;

        // Convert the GDI bitmap to a WPF ImageSource and make it cross-thread safe
        var hBitmap = bitmap.GetHbitmap ( ) ;

        try
        {
            var imageSource = Imaging.CreateBitmapSourceFromHBitmap ( hBitmap ,
                                                                      IntPtr.Zero ,
                                                                      Int32Rect.Empty ,
                                                                      BitmapSizeOptions.FromEmptyOptions ( ) ) ;
            imageSource.Freeze ( ) ;
            return imageSource ;
        }
        finally
        {
            // Avoid GDI handle leaks
            DeleteObject ( hBitmap ) ;
        }
    }

    private void PushIcons ( NotifyIcon taskbarIcon , ImageSource imageSource , int value )
    {
        if ( ! taskbarIcon.Dispatcher.CheckAccess ( ) )
        {
            taskbarIcon.Dispatcher.BeginInvoke ( new Action ( ( ) => PushIcons ( taskbarIcon ,
                                                                                 imageSource ,
                                                                                 value ) ) ) ;
            return ;
        }

        taskbarIcon.Icon        = imageSource ;
        taskbarIcon.TooltipText = $"Desk Height: {value} cm" ;
    }

    [ DllImport ( "gdi32.dll" ) ]
    [ return : MarshalAs ( UnmanagedType.Bool ) ]
    private static extern bool DeleteObject ( IntPtr hObject ) ;
}