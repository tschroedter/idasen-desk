using System.Diagnostics.CodeAnalysis ;
using Idasen.SystemTray.Win11.Views.Pages ;
using Idasen.SystemTray.Win11.Views.Windows ;
using Microsoft.Extensions.Hosting ;
using Wpf.Ui ;

namespace Idasen.SystemTray.Win11.Services ;

/// <summary>
///     Managed host of the application.
/// </summary>
[ ExcludeFromCodeCoverage ] // "Depends on WPF WindowCollection class"
public class ApplicationHostService ( INavigationWindow navigationWindow ) : IHostedService
{
    private static Window CurrentWindow =>
        Application.Current.MainWindow ?? throw new InvalidOperationException ( "Can't find the main window!" ) ;

    /// <summary>
    ///     Triggered when the application host is ready to start the service.
    /// </summary>
    /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
    public Task StartAsync ( CancellationToken cancellationToken )
    {
        cancellationToken.ThrowIfCancellationRequested ( ) ;
        return HandleActivationAsync ( cancellationToken ) ;
    }

    /// <summary>
    ///     Triggered when the application host is performing a graceful shutdown.
    /// </summary>
    /// <param name="cancellationToken">Indicates that the shutdown process should no longer be graceful.</param>
    public Task StopAsync ( CancellationToken cancellationToken )
    {
        return Task.CompletedTask ;
    }

    /// <summary>
    ///     Creates main window during activation.
    /// </summary>
    private async Task HandleActivationAsync ( CancellationToken cancellationToken )
    {
        var           currentWindow     = CurrentWindow ;
        var           contentRendered   = new TaskCompletionSource ( TaskCreationOptions.RunContinuationsAsynchronously ) ;
        EventHandler onContentRendered = ( _ , _ ) => contentRendered.TrySetResult ( ) ;
        currentWindow.ContentRendered += onContentRendered ;

        try
        {
            navigationWindow.ShowWindow ( ) ;
            navigationWindow.Navigate ( typeof ( SettingsPage ) ) ;

            await contentRendered.Task.WaitAsync ( cancellationToken ) ;

            var trayIcon = ( ( IdasenDeskWindow )currentWindow ).TrayIcon ;
            if ( ! trayIcon.IsRegistered )
                trayIcon.Register ( ) ;

            if ( ! trayIcon.IsRegistered )
                throw new InvalidOperationException ( "Failed to register the Idasen Desk tray icon." ) ;

            currentWindow.Visibility = Visibility.Hidden ;
        }
        finally
        {
            currentWindow.ContentRendered -= onContentRendered ;
        }
    }
}
