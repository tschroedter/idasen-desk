using System.IO ;
using Idasen.BluetoothLE.Core ;
using Idasen.Launcher ;
using Idasen.SystemTray.Win11.Utils ;
using JetBrains.Annotations ;
using Serilog ;

namespace Idasen.SystemTray.Win11.Settings
{
    public class SettingsManager
        : ISettingsManager
    {
        // todo inject ilogger
        private readonly ILogger _logger = LoggerProvider.CreateLogger(Constants.ApplicationName,
                                                                       Constants.LogFilename);

        public SettingsManager ( [ NotNull ] ICommonApplicationData commonApplicationData ,
                                 [ NotNull ] ISettingsStorage       settingsStorage )
        {
            Guard.ArgumentNotNull ( commonApplicationData ,
                                    nameof ( commonApplicationData ) ) ;
            Guard.ArgumentNotNull ( settingsStorage ,
                                    nameof ( settingsStorage ) ) ;

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

        private readonly ISettingsStorage _settingsStorage ;

        private Settings _current = new Settings ( ) ;
    }
}