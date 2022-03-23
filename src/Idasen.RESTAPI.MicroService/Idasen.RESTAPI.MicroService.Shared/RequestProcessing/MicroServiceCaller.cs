using System.Net ;
using Idasen.RESTAPI.MicroService.Shared.Interfaces ;
using Microsoft.AspNetCore.Http ;
using Microsoft.AspNetCore.Http.Extensions ;
using Serilog ;

namespace Idasen.RESTAPI.MicroService.Shared.RequestProcessing ;

public class MicroServiceCaller
    : IMicroServiceCaller
{
    private readonly ILogger                       _logger ;
    private readonly IMicroServiceSettingsProvider _provider ;

    public MicroServiceCaller ( ILogger                       logger ,
                                IMicroServiceSettingsProvider provider )
    {
        _logger   = logger ;
        _provider = provider ;
    }

    public async Task < bool > TryCall ( HttpContext httpContext )
    {
        try
        {
            _logger.Information ( $"Processing request for path '{httpContext.Request.Path}'" ) ;

            var response = await DoTryCall ( httpContext ) ;

            if ( response.StatusCode != HttpStatusCode.OK )
            {
                httpContext.Response.StatusCode = ( int )response.StatusCode ;

                return false ;
            }

            // Get the stream containing content returned by the server.
            var dataStream = await response.Content.ReadAsStreamAsync ( )
                                           .ConfigureAwait ( false ) ;
            // Open the stream using a StreamReader for easy access.
            var reader = new StreamReader ( dataStream ) ;
            // Read the content.
            var responseFromServer = await reader.ReadToEndAsync ( )
                                                 .ConfigureAwait ( false ) ;
            // Log the content.
            _logger.Information ( $"Response from '{httpContext.Request.Path}': '{responseFromServer}'" ) ;

            await httpContext.Response
                             .WriteAsync ( responseFromServer )
                             .ConfigureAwait ( false ) ;

            return true ;
        }
        catch ( Exception e )
        {
            _logger.Error ( e ,
                            $"Failed for url '{httpContext.Request.GetDisplayUrl ( )}'" ) ;

            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError ;

            return false ;
        }
    }

    private async Task < HttpResponseMessage > DoTryCall ( HttpContext context )
    {
        var path = context.Request.Path ;

        using var httpClient = new HttpClient ( ) ;

        if ( ! _provider.TryGetUri ( path ,
                                     out var uri ) )
        {
            var response = new HttpResponseMessage ( ) ;
            response.StatusCode = HttpStatusCode.NotFound ;

            return response ;
        }

        return await httpClient.GetAsync ( uri )
                               .ConfigureAwait ( false ) ;
    }
}