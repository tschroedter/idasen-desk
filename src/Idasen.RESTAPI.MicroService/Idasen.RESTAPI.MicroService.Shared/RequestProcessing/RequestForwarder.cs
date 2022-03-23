using Idasen.RESTAPI.MicroService.Shared.Interfaces ;
using Microsoft.AspNetCore.Http ;
using Microsoft.AspNetCore.Http.Extensions ;
using Serilog ;

namespace Idasen.RESTAPI.MicroService.Shared.RequestProcessing ;

public class RequestForwarder : IRequestForwarder
{
    private readonly IMicroServiceCaller _caller ;
    private readonly ILogger             _logger ;

    public RequestForwarder ( ILogger             logger ,
                              IMicroServiceCaller caller )
    {
        _logger = logger ;
        _caller = caller ;
    }

    public async Task Forward ( HttpContext httpContext )
    {
        _logger.Information ( $"Trying to forward call for '{httpContext.Request.GetDisplayUrl ( )}'" ) ;

        var success = await _caller.TryCall ( httpContext ) ;

        _logger.Information ( $"Success: {success} - Forwarding '{httpContext.Request.GetDisplayUrl ( )}'" ) ;
    }
}