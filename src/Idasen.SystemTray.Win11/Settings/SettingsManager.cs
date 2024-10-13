using System.IO ;
using Idasen.Launcher ;
using Idasen.SystemTray.Win11.Utils ;
using Serilog ;

namespace Idasen.SystemTray.Win11.Settings
{
    public class SettingsManager ( ICommonApplicationData commonApplicationData ,
                                   ISettingsStorage       settingsStorage )
        : ISettingsManager
    {
        // todo inject ILogger or use _container
        private readonly ILogger _logger = LoggerProvider.CreateLogger(Constants.ApplicationName,
                                                                       Constants.LogFilename);

        public ISettings CurrentSettings => _current ;

        public string SettingsFileName { get ; } = commonApplicationData.ToFullPath ( Constants.SettingsFileName ) ;

        public async Task Save ( )
        {
            await settingsStorage.SaveSettingsAsync ( SettingsFileName ,
                                                       _current ) ;
        }

        public async Task Load ( )
        {
            _current = await settingsStorage.LoadSettingsAsync ( SettingsFileName ) ;
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

        private Settings _current = new( ) ;
    }
}