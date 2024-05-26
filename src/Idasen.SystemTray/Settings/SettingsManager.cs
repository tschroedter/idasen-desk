using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization ;
using System.Threading.Tasks;
using Idasen.BluetoothLE.Core;
using Idasen.SystemTray.Interfaces;
using Idasen.SystemTray.Utils;
using JetBrains.Annotations;
using Serilog;

namespace Idasen.SystemTray.Settings
{
    public class SettingsManager
        : ISettingsManager
    {
        public SettingsManager ( [ NotNull ] ILogger                logger ,
                                 [ NotNull ] ICommonApplicationData commonApplicationData ,
                                 [ NotNull ] ISettingsStorage        settingsStorage)
        {
            Guard.ArgumentNotNull ( logger ,
                                    nameof ( logger ) ) ;
            Guard.ArgumentNotNull ( commonApplicationData ,
                                    nameof ( commonApplicationData ) ) ;
            Guard.ArgumentNotNull ( settingsStorage ,
                                    nameof ( settingsStorage ) ) ;

            _logger          = logger ;
            _settingsStorage = settingsStorage ;

            _settingsFolderName = commonApplicationData.FolderName ( ) ;
            _settingsFileName   = commonApplicationData.ToFullPath ( Constants.SettingsFileName ) ;

            _jsonOptions = new JsonSerializerOptions
                           {
                               DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull ,
                               PropertyNamingPolicy   = JsonNamingPolicy.CamelCase ,
                           } ;
        }

        public ISettings CurrentSettings => _current ?? new Settings (  );

        public async Task Save ( )
        {
            try
            {
                _logger.Debug ( $"Saving current setting [{_current}] to '{_settingsFileName}'" ) ;

                await _settingsStorage.SaveSettingsAsync (_settingsFileName,
                                                          _current) ;
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
                _current = await _settingsStorage.LoadSettingsAsync ( _settingsFileName ) ;
                
                _logger.Debug($"Settings loaded: {_current}");
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

        private void CreateDirectoryIfNotExist ( )
        {
            if ( ! Directory.Exists ( _settingsFolderName ) )
                Directory.CreateDirectory ( _settingsFolderName ) ;
        }

        private readonly ILogger               _logger ;
        private readonly ISettingsStorage       _settingsStorage ;
        private readonly string                _settingsFileName ;
        private readonly string                _settingsFolderName ;
        private readonly JsonSerializerOptions _jsonOptions ;

        private          Settings              _current = new Settings ( ) ;
    }
}