using System.Text ;
using Idasen.RESTAPI.MicroService.Shared.Interfaces ;
using Microsoft.AspNetCore.Http ;
using Microsoft.AspNetCore.Http.Extensions ;
using Serilog ;

namespace Idasen.RESTAPI.MicroService.Shared.RequestProcessing ;

public class SettingsCaller
    : ISettingsCaller
{
    private readonly ILogger _logger ;

    public SettingsCaller ( ILogger logger )
    {
        _logger = logger ;
    }

    public async Task < object? > TryCall ( HttpContext                   httpContext ,
                                            IMicroServiceSettingsProvider provider )
    {
        try
        {
            _logger.Information ( $"Processing request for path '{httpContext.Request.Path}'" ) ;

            var response = await DoTryCall ( provider ) ;

            if ( string.IsNullOrWhiteSpace ( response ) )
            {
                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError ;

                return false ;
            }

            await httpContext.Response.WriteAsync ( response )
                             .ConfigureAwait ( false ) ;

            return true ;
        }
        catch ( Exception e )
        {
            _logger.Error ( e ,
                            $"Failed for url '{httpContext.Request.GetDisplayUrl ( )}'" ) ;

            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError ;

            return null ;
        }
    }

    private static Task < string > DoTryCall ( IMicroServiceSettingsProvider provider )
    {
        var stringBuilder = new StringBuilder ( ) ;

        foreach ( var service in provider.MicroServices )
        {
            stringBuilder.AppendLine ( ( string? )$"['{service.Key}'] {service.Value}" ) ;
        }

        return Task.FromResult ( stringBuilder.ToString ( ) ) ;
    }
}