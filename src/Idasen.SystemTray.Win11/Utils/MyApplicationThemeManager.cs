using System.Diagnostics.CodeAnalysis ;
using Serilog ;
using Wpf.Ui.Appearance ;

namespace Idasen.SystemTray.Win11.Utils ;

[ ExcludeFromCodeCoverage ]
public class MyApplicationThemeManager ( ILogger logger ) : IApplicationThemeManager
{
    public ApplicationTheme GetAppTheme ( )
    {
        return ApplicationThemeManager.GetAppTheme ( ) ;
    }

    public void Apply ( ApplicationTheme theme )
    {
        try
        {
            // Fire-and-forget compatibility method: schedule apply and log any failures
            var task = ApplyAsync ( theme ) ;

            // Observe faults to avoid unobserved task exceptions
            task.ContinueWith ( t =>
                                {
                                    if ( t.Exception != null )
                                        logger.Error ( t.Exception ,
                                                       "Dispatcher theme apply task faulted for {Theme}" ,
                                                       theme ) ;
                                } ,
                                TaskContinuationOptions.OnlyOnFaulted ) ;
        }
        catch ( Exception ex )
        {
            // Log synchronous failures when starting to apply
            logger.Error ( ex ,
                           "Failed to start theme apply for {Theme}" ,
                           theme ) ;
        }
    }

    // New async API to allow callers to await theme application when necessary
    public Task ApplyAsync ( ApplicationTheme theme )
    {
        try
        {
            var dispatcher = Application.Current?.Dispatcher ;

            if ( dispatcher == null )
            {
                // No dispatcher, apply directly
                DoApply ( theme ) ;
                return Task.CompletedTask ;
            }

            if ( dispatcher.CheckAccess ( ) )
            {
                DoApply ( theme ) ;
                return Task.CompletedTask ;
            }

            // Dispatch DoApply to UI thread and return the operation task so callers can await it
            var op = dispatcher.InvokeAsync ( ( ) => DoApply ( theme ) ) ;

            // Attach continuation to log failures
            op.Task.ContinueWith ( t =>
                                   {
                                       if ( t.Exception != null )
                                           logger.Error ( t.Exception ,
                                                          "Failed to apply theme {Theme} on dispatcher thread." ,
                                                          theme ) ;
                                   } ,
                                   TaskContinuationOptions.OnlyOnFaulted ) ;

            return op.Task ;
        }
        catch ( Exception e )
        {
            // Log and rethrow with contextual information to satisfy static analysis
            logger.Error ( e ,
                           "Failed to schedule theme apply for {Theme}" ,
                           theme ) ;
            throw new InvalidOperationException ( $"Failed to schedule theme apply for {theme}" ,
                                                  e ) ;
        }
    }

    private void DoApply ( ApplicationTheme theme )
    {
        ApplicationThemeManager.Apply ( theme ) ;

        logger.Information ( "Successfully applied theme '{Theme}'..." ,
                             theme ) ;
    }
}