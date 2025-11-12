using Idasen.SystemTray.Win11.Interfaces ;
using Serilog ;
using Wpf.Ui.Appearance ;

namespace Idasen.SystemTray.Win11.Utils ;

public class ThemeRestoreWithDelayOnResume (
    ILogger                  logger ,
    ISettingsManager         settingsManager ,
    IApplicationThemeManager themeManager ) : IThemeRestoreWithDelayOnResume
{
    private string ThemeName => settingsManager.CurrentSettings.AppearanceSettings.ThemeName ;

    public async Task ApplyWithDelayAsync ( CancellationToken token )
    {
        try
        {
            if ( string.IsNullOrWhiteSpace ( ThemeName ) )
            {
                logger.Warning ( "Theme is not set!" ) ;

                return ;
            }

            for ( var i = 0 ; i < 10 ; i ++ )
            {
                await Task.Delay ( 250 ,
                                   token ).ConfigureAwait ( false ) ; // small wait for OS to settle

                if ( Enum.TryParse < ApplicationTheme > ( ThemeName ,
                                                          true ,
                                                          out var theme ) )
                    try
                    {
                        await themeManager.ApplyAsync ( theme ) ;
                    }
                    catch ( Exception ex )
                    {
                        logger.Error ( ex ,
                                       "Failed to reapply theme after display/user preference change" ) ;
                    }

                var current = themeManager.GetAppTheme ( ) ;

                if ( current == theme )
                    break ;
            }

            if ( themeManager.GetAppTheme ( ).ToString ( ) != ThemeName )
                logger.Warning ( "After retries, the applied theme does not match the configured theme '{ThemeName}'" ,
                                 ThemeName ) ;
        }
        catch ( Exception ex )
        {
            logger.Error ( ex ,
                           "ReapplyConfiguredThemeWithDelay failed" ) ;
        }
    }
}