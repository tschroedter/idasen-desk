using Idasen.SystemTray.Win11.Interfaces ;
using Microsoft.Win32 ;
using Serilog ;

namespace Idasen.SystemTray.Win11.Utils ;

public partial class ThemeRestoreOnResume (
    ILogger                        logger ,
    IPowerEvents                   powerEvents ,
    IThemeRestoreWithDelayOnResume themeRestoreWithDelay )
    : IDisposable
{
    private bool _disposed ;

    public void Dispose ( )
    {
        Dispose ( true ) ;
        GC.SuppressFinalize ( this ) ;
    }

    public void Initialize ( )
    {
        powerEvents.PowerModeChanged += OnPowerModeChanged ;

        // React when display or user preferences change (eGPU/monitor changes often fire these)
        SystemEvents.DisplaySettingsChanged += OnDisplaySettingsChanged ;
        SystemEvents.UserPreferenceChanged  += OnUserPreferenceChanged ;
    }

    protected virtual void Dispose ( bool disposing )
    {
        if ( _disposed )
            return ;

        if ( disposing )
        {
            powerEvents.PowerModeChanged -= OnPowerModeChanged ;
            powerEvents.Dispose ( ) ;

            SystemEvents.DisplaySettingsChanged -= OnDisplaySettingsChanged ;
            SystemEvents.UserPreferenceChanged  -= OnUserPreferenceChanged ;
        }

        _disposed = true ;
    }

    private void OnPowerModeChanged ( object ? sender , PowerModeChangedEventArgs e )
    {
        logger.Information ( "Power mode changed: {Mode}" ,
                             e.Mode ) ;

        ReapplyConfiguredThemeWithDelay ( ) ;
    }

    // Handle display/user preference changes (e.g. when connecting eGPU/monitor)
    private void OnDisplaySettingsChanged ( object ? sender , EventArgs e )
    {
        logger.Information ( "Display settings changed." ) ;

        ReapplyConfiguredThemeWithDelay ( ) ;
    }

    private void OnUserPreferenceChanged ( object ? sender , UserPreferenceChangedEventArgs e )
    {
        // Only react 2 categories likely to affect theme (optional)
        if ( e.Category is UserPreferenceCategory.General or UserPreferenceCategory.Color )
        {
            logger.Information ( "User preferences changed: {Category}" ,
                                 e.Category ) ;

            ReapplyConfiguredThemeWithDelay ( ) ;
        }
    }

    private void ReapplyConfiguredThemeWithDelay ( )
    {
        _ = Task.Run ( async ( ) => { await themeRestoreWithDelay.ApplyWithDelayAsync ( CancellationToken.None ) ; } ) ;
    }
}