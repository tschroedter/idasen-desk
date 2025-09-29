using System.Diagnostics.CodeAnalysis ;
using System.Drawing ;
using System.Drawing.Drawing2D ;
using System.Drawing.Text ;
using System.Globalization ;
using System.Runtime.InteropServices ;
using System.Windows.Interop ;
using System.Windows.Media ;
using System.Windows.Media.Imaging ;
using Idasen.BluetoothLE.Core ;
using Idasen.SystemTray.Win11.Interfaces ;
using Wpf.Ui.Tray.Controls ;
using Color = System.Drawing.Color ;
using Pen = System.Drawing.Pen ;
using DFont = System.Drawing.Font ;
using DFontStyle = System.Drawing.FontStyle ;
using DGraphics = System.Drawing.Graphics ;
using DGraphicsUnit = System.Drawing.GraphicsUnit ;
using DPixelFormat = System.Drawing.Imaging.PixelFormat ;
using DSolidBrush = System.Drawing.SolidBrush ;

namespace Idasen.SystemTray.Win11.Utils.Icons ;

[ ExcludeFromCodeCoverage ]
public class DynamicIconCreator : IDynamicIconCreator
{
    private const           int    IconHeight     = 16 ; // base logical height at 100% DPI
    private const           int    FontSize       = 8 ;  // base logical font size (px) at 100% DPI (used as fallback)
    private const           string FontFamily     = "Consolas" ;
    private static readonly Color  BrushDarkBlue  = ColorTranslator.FromHtml ( "#FF0048A3" ) ;
    private static readonly Color  BrushLightBlue = ColorTranslator.FromHtml ( "#FF0098F3" ) ;

    public void Update ( NotifyIcon taskbarIcon , int height )
    {
        Guard.ArgumentNotNull ( taskbarIcon ,
                                nameof ( taskbarIcon ) ) ;

        // Get per-monitor DPI scale for crisp rendering in the tray
        var dpi = VisualTreeHelper.GetDpi ( taskbarIcon ) ;

        var image = CreateImageSource ( height ,
                                        dpi.DpiScaleX ,
                                        dpi.DpiScaleY ) ;

        PushIcons ( taskbarIcon ,
                    image ,
                    height ) ;
    }

    private static BitmapSource CreateImageSource ( int height , double scaleX , double scaleY )
    {
        // Height text
        var text = height.ToString ( CultureInfo.InvariantCulture ) ;

        // Compute pixel bounds based on DPI and text length for better readability
        var pixelHeight = Math.Max ( 16 ,
                                     ( int )Math.Round ( IconHeight * scaleY ) ) ;

        // Make font fill most of the icon height for readability
        var fontSizePx = ( int )Math.Max ( 10 ,
                                           Math.Floor ( pixelHeight * 0.90 ) ) ;

        // Estimate width based on monospace character width (~0.6 of height), plus small padding
        var estimatedCharWidth = Math.Max ( 8 ,
                                            ( int )Math.Round ( fontSizePx * 0.60 ) ) ;
        var pixelWidth = Math.Max ( 16 ,
                                    estimatedCharWidth * text.Length + 4 ) ;

        using var bitmap = new Bitmap ( pixelWidth ,
                                        pixelHeight ,
                                        DPixelFormat.Format32bppPArgb ) ;
        bitmap.SetResolution ( ( float )( 96.0 * scaleX ) ,
                               ( float )( 96.0 * scaleY ) ) ;

        using var graphics = DGraphics.FromImage ( bitmap ) ;
        graphics.SmoothingMode     = SmoothingMode.AntiAlias ;
        graphics.PixelOffsetMode   = PixelOffsetMode.HighQuality ;
        graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit ;
        graphics.Clear ( Color.Transparent ) ;

        using var pen = new Pen ( BrushDarkBlue ,
                                  1.0f ) ;
        using var brush = new DSolidBrush ( BrushLightBlue ) ;
        using var font = new DFont ( FontFamily ,
                                     fontSizePx ,
                                     DFontStyle.Bold ,
                                     DGraphicsUnit.Pixel ) ;

        // Draw top and bottom horizontal lines across the full scaled width
        graphics.DrawLine ( pen ,
                            0 ,
                            0 ,
                            pixelWidth ,
                            0 ) ;
        graphics.DrawLine ( pen ,
                            0 ,
                            pixelHeight - 1 ,
                            pixelWidth ,
                            pixelHeight - 1 ) ;

        // Draw the height value string (slightly inset)
        graphics.DrawString ( text ,
                              font ,
                              brush ,
                              new PointF ( 1f ,
                                           Math.Max ( 0f ,
                                                      pixelHeight - fontSizePx - 1f ) ) ) ;

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

    private static void PushIcons ( NotifyIcon taskbarIcon , ImageSource imageSource , int value )
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