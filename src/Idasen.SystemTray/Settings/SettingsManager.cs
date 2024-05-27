using System.IO ;
using System.Threading.Tasks ;
using Idasen.BluetoothLE.Core ;
using Idasen.SystemTray.Interfaces ;
using Idasen.SystemTray.Utils ;
using JetBrains.Annotations ;
using Serilog ;

namespace Idasen.SystemTray.Settings
{
    public class SettingsManager
        : ISettingsManager
    {
        public SettingsManager ( [ NotNull ] ILogger                logger ,
                                 [ NotNull ] ICommonApplicationData commonApplicationData ,
                                 [ NotNull ] ISettingsStorage       settingsStorage )
        {
            Guard.ArgumentNotNull ( logger ,
                                    nameof ( logger ) ) ;
            Guard.ArgumentNotNull ( commonApplicationData ,
                                    nameof ( commonApplicationData ) ) ;
            Guard.ArgumentNotNull ( settingsStorage ,
                                    nameof ( settingsStorage ) ) ;

            _logger           = logger ;
            _settingsStorage  = settingsStorage ;

            SettingsFileName = commonApplicationData.ToFullPath ( Constants.SettingsFileName ) ;
        }

        public ISettings CurrentSettings
        {
            get => _current ?? new Settings ( ) ;
        }

        public string SettingsFileName { get ; }

        public async Task Save ( )
        {
            await _settingsStorage.SaveSettingsAsync ( SettingsFileName ,
                                                       _current ) ;
        }

        public async Task Load ( )
        {
            _current = await _settingsStorage.LoadSettingsAsync ( SettingsFileName ) ;
        }

        public async Task < bool > UpgradeSettings ( )
        {
            if ( ! File.Exists ( SettingsFileName ) )
                return true ;

            var settings = await File.ReadAllTextAsync ( SettingsFileName )
                                     .ConfigureAwait ( false ) ;

            if ( ! settings.Contains ( nameof ( Settings.NotificationsEnabled ) ) )
            {
                await AddMissingSettingsNotificationsEnabled ( ) ;
            }
            
            return false ;
        }

        private async Task AddMissingSettingsNotificationsEnabled ( )
        {
            _logger.Debug ( $"Add missing setting "                        +
                            $"{nameof ( Settings.NotificationsEnabled )} " +
                            $"to current settings from '{SettingsFileName}'" ) ;

            await Load ( ).ConfigureAwait ( false ) ;

            _current.NotificationsEnabled = Constants.NotificationsEnabled ;

            await Save ( ) ;
        }

        private readonly ILogger          _logger ;
        private readonly ISettingsStorage _settingsStorage ;

        private Settings _current = new Settings ( ) ;
    }
}