using System.Drawing ;
using Idasen.BluetoothLE.Core ;
using Wpf.Ui.Tray.Controls ;

namespace Idasen.SystemTray.Win11
{
    public class DynamicIconCreator : IDynamicIconCreator
    {
        public void Update ( NotifyIcon taskbarIcon ,
                             int           height )
        {
            Guard.ArgumentNotNull ( taskbarIcon ,
                                    nameof ( taskbarIcon ) ) ;

            PushIcons ( taskbarIcon ,
                        CreateIcon ( height ) ,
                        height ) ;
        }

        private Icon CreateIcon ( int height )
        {
            var width = height >= 100
                            ? 24
                            : 16 ;

            using var pen   = new Pen ( _brushDarkBlue ) ;
            using var brush = new SolidBrush ( _brushLightBlue ) ;
            using var font = new Font ( "Consolas" ,
                                        8 ) ;

            using var bitmap = new Bitmap ( width ,
                                            16 ) ;

            using var graph = Graphics.FromImage ( bitmap ) ;

            //draw two horizontal lines
            graph.DrawLine ( pen ,
                             0 ,
                             15 ,
                             width ,
                             15 ) ;
            graph.DrawLine ( pen ,
                             0 ,
                             0 ,
                             width ,
                             0 ) ;

            //draw the string including the value at origin
            graph.DrawString ( $"{height}" ,
                               font ,
                               brush ,
                               new PointF ( - 1 ,
                                            1 ) ) ;


            var icon = bitmap.GetHicon ( ) ;

            //create a new icon from the handle
            return Icon.FromHandle ( icon ) ;
        }

        private void PushIcons (NotifyIcon taskbarIcon ,
                                Icon       icon ,
                                int        value )
        {
            if ( ! taskbarIcon.Dispatcher.CheckAccess ( ) )
            {
                taskbarIcon.Dispatcher
                           .BeginInvoke ( new Action ( ( ) => PushIcons ( taskbarIcon ,
                                                                          icon ,
                                                                          value ) ) ) ;

                return ;
            }

            //push the icons to the system tray
            taskbarIcon.Icon        = icon.ToImageSource() ;
            // todo taskbarIcon.ToolTipText = $"Desk Height: {value}cm" ;
        }

        private readonly Color _brushDarkBlue  = ColorTranslator.FromHtml ( "#FF0048A3" ) ;
        private readonly Color _brushLightBlue = ColorTranslator.FromHtml ( "#FF0098F3" ) ;
    }
}
