using System.Diagnostics ;
using System.Diagnostics.CodeAnalysis ;
using System.IO ;
using Idasen.SystemTray.Win11.Interfaces ;
using Microsoft.Extensions.Configuration ;
using Serilog ;

namespace Idasen.SystemTray.Win11.Services ;

public class DonateService (
    IConfiguration  configuration ,
    ILogger         logger ,
    IProcessLauncher processLauncher ) : IDonateService
{
#pragma warning disable S1075 // URIs should not be hardcoded - This is a fallback URL when configuration is missing
    private const string DefaultDonateUrl = "https://www.paypal.com/donate/?hosted_button_id=KAWJDNVJTD7SJ" ;
#pragma warning restore S1075

    public void OpenDonateUrl ( )
    {
        try
        {
            var donateUrl = configuration [ "DonateUrl" ] ;

            if ( string.IsNullOrEmpty ( donateUrl ) )
            {
                logger.Debug ( "DonateUrl is not configured. Using built-in default donate URL." ) ;
                donateUrl = DefaultDonateUrl ;
            }
            else if ( ! TryCreateDonateUri ( donateUrl , out _ ) )
            {
                logger.Warning ( "DonateUrl is invalid or not an absolute HTTP/HTTPS URI, using default URL" ) ;
                donateUrl = DefaultDonateUrl ;
            }

            if ( ! TryCreateDonateUri ( donateUrl , out var donateUri ) )
            {
                logger.Error ( "Unable to open donate URL because neither the configured value nor the fallback URL is a valid absolute HTTP/HTTPS URI." ) ;
                return ;
            }

            logger.Information ( "Donate button clicked. Using DonateUrl: {DonateUrl}" , donateUri.AbsoluteUri ) ;

            OpenUrlInBrowser ( donateUri.AbsoluteUri ) ;
        }
        catch ( Exception ex )
        {
            logger.Error ( ex ,
                           "An error occurred while trying to open the donate URL." ) ;
        }
    }

    private static bool TryCreateDonateUri ( string url , [ NotNullWhen ( true ) ] out Uri? donateUri )
    {
        if ( Uri.TryCreate ( url , UriKind.Absolute , out var parsedUri )
             && ( parsedUri.Scheme == Uri.UriSchemeHttp || parsedUri.Scheme == Uri.UriSchemeHttps ) )
        {
            donateUri = parsedUri ;
            return true ;
        }

        donateUri = null ;
        return false ;
    }

    private void OpenUrlInBrowser ( string url )
    {
        try
        {
            // Primary method: Direct Process.Start with UseShellExecute
            logger.Information ( "Attempting to open URL: {Url}" , url ) ;

            var primaryInfo = new ProcessStartInfo
            {
                FileName        = url ,
                UseShellExecute = true
            } ;

            if ( processLauncher.TryStart ( primaryInfo ) )
            {
                logger.Information ( "Successfully opened URL in browser" ) ;
                return ;
            }

            logger.Warning ( "Process.Start returned null, trying fallback method" ) ;
        }
        catch ( System.ComponentModel.Win32Exception ex )
        {
            logger.Warning ( ex , "Primary method failed with Win32Exception, trying fallback" ) ;
        }
        catch ( Exception ex )
        {
            logger.Warning ( ex , "Primary method failed, trying fallback" ) ;
        }

        // Fallback method: Use cmd /c start
        try
        {
            logger.Information ( "Trying fallback method with cmd /c start" ) ;

            var systemRoot = Environment.GetEnvironmentVariable ( "SystemRoot" ) ?? "C:\\Windows" ;
            var cmdPath    = Path.Combine ( systemRoot , "System32" , "cmd.exe" ) ;

            var fallbackInfo = new ProcessStartInfo
            {
                FileName        = cmdPath ,
                Arguments       = $"/c start \"\" \"{url}\"" ,
                UseShellExecute = false ,
                CreateNoWindow  = true
            } ;

            var ( started , exitCode ) = processLauncher.StartAndWait ( fallbackInfo , 5000 ) ;

            if ( ! started )
            {
                logger.Error ( "Both primary and fallback methods failed to open URL" ) ;
                return ;
            }

            if ( exitCode == null )
            {
                logger.Error ( "Fallback method did not complete within the expected time" ) ;
                return ;
            }

            if ( exitCode == 0 )
            {
                logger.Information ( "Successfully opened URL using fallback method" ) ;
            }
            else
            {
                logger.Error ( "Fallback method failed with exit code {ExitCode}" , exitCode ) ;
            }
        }
        catch ( Exception ex )
        {
            logger.Error ( ex , "Fallback method also failed" ) ;
        }
    }
}
