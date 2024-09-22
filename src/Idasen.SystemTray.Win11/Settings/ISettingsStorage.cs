namespace Idasen.SystemTray.Win11.Settings
{
    public interface ISettingsStorage
    {
        Task < Settings > LoadSettingsAsync ( string settingsFileName ) ;
        Task              SaveSettingsAsync ( string settingsFileName , Settings settings ) ;
    }
}