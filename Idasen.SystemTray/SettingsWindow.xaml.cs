using System.Windows.Input ;

namespace Idasen.SystemTray
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class SettingsWindow
        : ISettingsWindow
    {
        public SettingsWindow ( )
        {
            InitializeComponent ( ) ;
        }

        public uint StandingHeightInCm => _converter.TryConvertToUInt ( Standing.Value ,
                                                                        SettingsConstants.DefaultHeightStandingInCm ) ;

        public uint SeatingHeightInCm => _converter.TryConvertToUInt ( Seating.Value ,
                                                                       SettingsConstants.DefaultHeightSeatingInCm ) ;

        private void ImageClose_MouseDown ( object               sender ,
                                            MouseButtonEventArgs e )
        {
            Close ( ) ;
        }

        private readonly IDoubleToUIntConverter _converter = new DoubleToUIntConverter ( ) ;
    }
}