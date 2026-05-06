using System.Diagnostics ;
using System.Diagnostics.CodeAnalysis ;
using System.IO ;
using System.Windows.Input ;
using Idasen.SystemTray.Win11.ViewModels.Pages ;
using Microsoft.Extensions.Configuration ;
using Serilog ;
using Wpf.Ui.Abstractions.Controls ;

namespace Idasen.SystemTray.Win11.Views.Pages ;

[ ExcludeFromCodeCoverage ]
public partial class HomePage : INavigableView < DashboardViewModel >
{
#pragma warning disable S1075 // URIs should not be hardcoded - This is a fallback URL when configuration is missing
    private const string DefaultDonateUrl = "https://www.paypal.com/donate/?hosted_button_id=KAWJDNVJTD7SJ" ;
#pragma warning restore S1075

    private readonly IConfiguration _configuration ;
    private readonly ILogger        _logger ;

    public HomePage ( DashboardViewModel viewModel , 
                      IConfiguration configuration ,
                      ILogger logger)
    {
        ViewModel      = viewModel ;
        DataContext    = this ;
        _configuration = configuration ;
        _logger   = logger ;

        InitializeComponent ( ) ;
    }

    public DashboardViewModel ViewModel { get ; }

    private void DonateImage_OnClick ( object sender , MouseButtonEventArgs e )
    {
        try
        {
            var donateUrl = _configuration [ "DonateUrl" ] ;

            if ( string.IsNullOrEmpty ( donateUrl ) )
            {
                _logger.Warning ( "DonateUrl is not configured in appsettings.json, using default URL" );
                donateUrl = DefaultDonateUrl ;
            }

            _logger.Information ( "Donate button clicked. Using DonateUrl: {DonateUrl}" , donateUrl );

            OpenUrlInBrowser ( donateUrl ) ;
        }
        catch (Exception ex)
        {
            _logger.Error ( ex , 
                            "An error occurred while trying to open the donate URL." );
        }
    }

    private void OpenUrlInBrowser ( string url )
    {
        try
        {
            // Primary method: Direct Process.Start with UseShellExecute
            _logger.Information ( "Attempting to open URL: {Url}" , url );

            var processStartInfo = new ProcessStartInfo
            {
                FileName = url ,
                UseShellExecute = true
            };

            var process = Process.Start ( processStartInfo ) ;

            if ( process != null )
            {
                _logger.Information ( "Successfully opened URL in browser" );
                return ;
            }

            _logger.Warning ( "Process.Start returned null, trying fallback method" );
        }
        catch ( System.ComponentModel.Win32Exception ex )
        {
            _logger.Warning ( ex , "Primary method failed with Win32Exception, trying fallback" );
        }
        catch ( Exception ex )
        {
            _logger.Warning ( ex , "Primary method failed, trying fallback" );
        }

        // Fallback method: Use cmd /c start
        try
        {
            _logger.Information ( "Trying fallback method with cmd /c start" );

            var systemRoot = Environment.GetEnvironmentVariable ( "SystemRoot" ) ?? "C:\\Windows" ;
            var cmdPath = Path.Combine ( systemRoot , "System32" , "cmd.exe" ) ;

            var processStartInfo = new ProcessStartInfo
            {
                FileName = cmdPath ,
                Arguments = $"/c start \"\" \"{url}\"" ,
                UseShellExecute = false ,
                CreateNoWindow = true
            };

            var process = Process.Start ( processStartInfo ) ;

            if ( process != null )
            {
                _logger.Information ( "Successfully opened URL using fallback method" );
            }
            else
            {
                _logger.Error ( "Both primary and fallback methods failed to open URL" );
            }
        }
        catch ( Exception ex )
        {
            _logger.Error ( ex , "Fallback method also failed" );
        }
    }
}