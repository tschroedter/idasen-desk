using System.Diagnostics ;
using System.IO ;
using System.Text ;
using Idasen.SystemTray.Win11.Interfaces ;
using Microsoft.Extensions.Configuration ;

namespace Idasen.SystemTray.Win11.Utils ;

public class IdasenConfigurationProvider : IIdasenConfigurationProvider // todo can this class be removed
{
    public IConfigurationRoot GetConfiguration ( )
    {
        const string appsettingsJson = "appsettings.json" ;

        IConfigurationRoot configurationRoot ;

        var builder  = new StringBuilder ( ) ;
        var basePath = GetBasePath ( ) ;
        var fullPath = Path.Combine ( basePath ,
                                      appsettingsJson ) ;

        builder.AppendLine ( $"Checking if '{fullPath}' exists..." ) ;

        if ( File.Exists ( fullPath ) )
        {
            builder.AppendLine ( $"Loading settings from file '{fullPath}'..." ) ;

            configurationRoot = new ConfigurationBuilder ( ).SetBasePath ( basePath )
                                                            .AddJsonFile ( appsettingsJson )
                                                            .Build ( ) ;
        }
        else
        {
            builder.AppendLine ( $"...no, '{fullPath}' does not exists." ) ;
            builder.AppendLine ( "Using default settings..." ) ;

            configurationRoot = new ConfigurationBuilder ( ).AddJsonFile ( appsettingsJson )
                                                            .Build ( ) ;
        }

        builder.AppendLine ( "Using the following configuration:" ) ;

        builder.AppendLine ( configurationRoot.GetDebugView ( ) ) ;

        LogConfigurationSelection ( basePath ,
                                    builder ) ;

        return configurationRoot ;
    }

    private static string GetBasePath ( )
    {
        using var processModule = Process.GetCurrentProcess ( ).MainModule ;

        return Path.GetDirectoryName ( processModule?.FileName ) ??
               throw new InvalidOperationException ( "Couldn't get directory name from entry assembly" ) ;
    }

    private static void LogConfigurationSelection ( string        basePath ,
                                                    StringBuilder builder )
    {
        try
        {
            var logFolder = Path.Combine ( basePath ,
                                           "logs" ) ;

            if ( ! Directory.Exists ( logFolder ) )
                Directory.CreateDirectory ( logFolder ) ;

            var configLog = Path.Combine ( logFolder ,
                                           "config.log" ) ;

            if ( File.Exists ( configLog ) )
                File.Delete ( configLog ) ;

            File.WriteAllText ( configLog ,
                                builder.ToString ( ) ) ;
        }
        catch ( Exception e )
        {
            Console.WriteLine ( $"Failed to create configuration log file because {e.Message}" ) ;
        }
    }
}