using System ;
using System.IO ;
using System.Text.Json ;
using System.Threading.Tasks ;
using System.Windows.Input ;
using Idasen.SystemTray.Converters ;
using Idasen.SystemTray.Interfaces ;
using JetBrains.Annotations ;

namespace Idasen.SystemTray
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class SettingsWindow
        : ISettingsWindow
    {
        public SettingsWindow ( [ NotNull ] ISettingsManager manager )
        {
            // Guard.ArgumentNotNull ( manager ,
            //                         nameof ( manager ) ) ; todo
            _manager = manager ;

            InitializeComponent ( ) ;
        }

        private void ImageClose_MouseDown ( object               sender ,
                                            MouseButtonEventArgs e )
        {
            Close ( ) ;

            StoreSettings ( ) ;
        }

        private void StoreSettings ( )
        {
            _manager.StandingHeightInCm = _converter.TryConvertToUInt ( Standing.Value ,
                                                                        Constants.DefaultHeightStandingInCm ) ;
            _manager.SeatingHeightInCm = _converter.TryConvertToUInt ( Seating.Value ,
                                                                       Constants.DefaultHeightSeatingInCm ) ;
            _manager.Save ( ) ;
        }

        private readonly IDoubleToUIntConverter _converter = new DoubleToUIntConverter ( ) ;
        private readonly ISettingsManager       _manager ;

        private void SettingsWindow_OnClosed ( object?   sender ,
                                               EventArgs e )
        {
            StoreSettings();
        }
    }

    public interface ISettingsManager
    {
        uint   StandingHeightInCm { get ; set ; }
        uint   SeatingHeightInCm  { get ; set ; }
        string SettingsFileName   { get ; }
        Task   Save ( ) ;
        Task   Load ( ) ;
    }

    public class SettingsManager
        : ISettingsManager
    {
        public SettingsManager ( )
        {
            SettingsFolderName = CreateFullPathSettingsFolderName ( ) ;
            SettingsFileName   = CreateFullPathSettingsFileName ( ) ;
        }

        public uint StandingHeightInCm
        {
            get => _current.StandingHeightInCm ;
            set => _current.StandingHeightInCm = value ;
        }

        public uint SeatingHeightInCm
        {
            get => _current.SeatingHeightInCm ;
            set => _current.SeatingHeightInCm = value ;
        }

        public async Task Save ( )
        {
            try
            {
                if ( ! Directory.Exists ( SettingsFolderName ) )
                    Directory.CreateDirectory ( SettingsFolderName ) ;

                await using var stream = File.Create ( SettingsFileName ) ;

                await JsonSerializer.SerializeAsync ( stream ,
                                                      _current ) ;
            }
            catch ( Exception )
            {
                // todo
            }
        }

        public async Task Load ( )
        {
            try
            {
                if ( ! File.Exists ( SettingsFileName ) )
                    return ;

                await using var openStream = File.OpenRead ( SettingsFileName ) ;

                _current = await JsonSerializer.DeserializeAsync < Settings > ( openStream ) ;
            }
            catch ( Exception )
            {
                // todo
            }
        }

        public string SettingsFileName   { get ; }
        public string SettingsFolderName { get ; }

#pragma warning disable SecurityIntelliSenseCS // MS Security rules violation
        public string CreateFullPathSettingsFileName ( )
        {
            var fileName = Path.Combine ( CreateFullPathSettingsFolderName ( ) ,
                                          Constants.SettingsFileName ) ;
            return fileName ;
        }
#pragma warning restore SecurityIntelliSenseCS // MS Security rules violation

#pragma warning disable SecurityIntelliSenseCS // MS Security rules violation
        private string CreateFullPathSettingsFolderName ( )
        {
            var appData = Environment.GetFolderPath ( Environment.SpecialFolder.ApplicationData ) ;
            var folderName = Path.Combine ( appData ,
                                            Constants.ApplicationName ) ;

            return folderName ;
        }
#pragma warning restore SecurityIntelliSenseCS // MS Security rules violation

        private Settings _current = new Settings ( ) ;
    }

    public class Settings
    {
        public uint StandingHeightInCm { get ; set ; } = Constants.DefaultHeightStandingInCm ;
        public uint SeatingHeightInCm  { get ; set ; } = Constants.DefaultHeightSeatingInCm ;
    }
}