using System ;
using System.IO ;
using System.Text.Json ;
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
        public SettingsManager ( [ NotNull ] ILogger logger )
        {
            Guard.ArgumentNotNull ( logger ,
                                    nameof ( logger ) ) ;

            _logger = logger ;

            _settingsFolderName = CreateFullPathSettingsFolderName ( ) ;
            _settingsFileName   = CreateFullPathSettingsFileName ( ) ;
        }

        public ISettings CurrentSettings => _current ;

        public async Task Save ( )
        {
            _logger.Debug ( $"Saving current setting [{_current}] to '{_settingsFileName}'" ) ;

            try
            {
                if ( ! Directory.Exists ( _settingsFolderName ) )
                    Directory.CreateDirectory ( _settingsFolderName ) ;

                await using var stream = File.Create ( _settingsFileName ) ;

                await JsonSerializer.SerializeAsync ( stream ,
                                                      _current ) ;
            }
            catch ( Exception e )
            {
                _logger.Error ( e ,
                                $"Failed to save settings in file '{_settingsFileName}'" ) ;
            }
        }

        public async Task Load ( )
        {
            _logger.Debug ( $"Loading setting from '{_settingsFileName}'" ) ;

            try
            {
                if ( ! File.Exists ( _settingsFileName ) )
                    return ;

                await using var openStream = File.OpenRead ( _settingsFileName ) ;

                _current = await JsonSerializer.DeserializeAsync < Settings > ( openStream ) ;

                _logger.Debug ( $"Settings loaded: {_current}" ) ;
            }
            catch ( Exception e )
            {
                _logger.Error ( e ,
                                "Failed to load settings" ) ;
            }
        }

        public async Task UpgradeSettings ( )
        {
            _logger.Debug($"Check current setting from '{_settingsFileName}'");

            try
            {
                if(!File.Exists (_settingsFileName))
                    return;

                var settings = await File.ReadAllTextAsync ( _settingsFileName )
                                         .ConfigureAwait ( false ) ;

                if (!settings.Contains ( nameof(Settings.NotificationsEnabled) ) )
                {
                    await AddMissingSettingsNotificationsEnabled ( ) ;
                }

                _logger.Debug($"Upgrade check completed for current setting from '{_settingsFileName}'");
            }
            catch ( Exception e )
            {
                _logger.Error(e,
                              "Failed to upgrade settings");
            }
        }

        private async Task AddMissingSettingsNotificationsEnabled ( )
        {
            _logger.Debug ( $"Add missing setting "                        +
                            $"{nameof ( Settings.NotificationsEnabled )} " +
                            $"to current settings from '{_settingsFileName}'" ) ;

            await Load ( ).ConfigureAwait ( false ) ;

            _current.NotificationsEnabled = Constants.NotificationsEnabled ;

            await Save ( ) ;
        }

        private readonly ILogger _logger ;

        private readonly string _settingsFileName ;

        private readonly string _settingsFolderName ;

        private Settings _current = new Settings ( ) ;

#pragma warning disable SecurityIntelliSenseCS // MS Security rules violation
        public string CreateFullPathSettingsFileName ( )
        {
            var fileName = Path.Combine ( CreateFullPathSettingsFolderName ( ) ,
                                          Constants.SettingsFileName ) ;
            return fileName ;
        }

        private string CreateFullPathSettingsFolderName ( )
        {
            var appData = Environment.GetFolderPath ( Environment.SpecialFolder.CommonApplicationData ) ;
            var folderName = Path.Combine ( appData ,
                                            Constants.ApplicationName ) ;

            return folderName ;
        }
#pragma warning restore SecurityIntelliSenseCS // MS Security rules violation
    }
}