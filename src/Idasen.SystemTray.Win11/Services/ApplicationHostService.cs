﻿using System.Diagnostics.CodeAnalysis ;
using Idasen.SystemTray.Win11.Views.Pages ;
using Idasen.SystemTray.Win11.Views.Windows ;
using Microsoft.Extensions.Hosting ;
using Wpf.Ui ;

namespace Idasen.SystemTray.Win11.Services ;

/// <summary>
///     Managed host of the application.
/// </summary>
[ExcludeFromCodeCoverage] // "Depends on WPF WindowCollection class"
public class ApplicationHostService ( IServiceProvider serviceProvider ) : IHostedService
{
    private INavigationWindow ? _navigationWindow ;

    private static Window CurrentWindow =>
        Application.Current.MainWindow ?? throw new Exception ( "Can't find the main window!" ) ;

    /// <summary>
    ///     Triggered when the application host is ready to start the service.
    /// </summary>
    /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
    public async Task StartAsync ( CancellationToken cancellationToken )
    {
        await HandleActivationAsync ( ) ;
    }

    /// <summary>
    ///     Triggered when the application host is performing a graceful shutdown.
    /// </summary>
    /// <param name="cancellationToken">Indicates that the shutdown process should no longer be graceful.</param>
    public async Task StopAsync ( CancellationToken cancellationToken )
    {
        await Task.CompletedTask ;
    }

    /// <summary>
    ///     Creates main window during activation.
    /// </summary>
    private async Task HandleActivationAsync ( )
    {
        if ( ! Application.Current.Windows.OfType < IdasenDeskWindow > ( ).Any ( ) )
        {
            _navigationWindow = (
                                    serviceProvider.GetService ( typeof ( INavigationWindow ) ) as INavigationWindow
                                )! ;
            _navigationWindow!.ShowWindow ( ) ;

            _navigationWindow.Navigate ( typeof ( SettingsPage ) ) ;

            CurrentWindow.Visibility = Visibility.Hidden ;
        }

        await Task.CompletedTask ;
    }
}