using Microsoft.AspNetCore.Http.Extensions ;
using ILogger = Serilog.ILogger ;

namespace Idasen.RESTAPI.Desk.RequestProcessing ;

public class RequestForwarder : IRequestForwarder
{
    private readonly ILogger                       _logger ;
    private readonly IMicroServiceCaller           _caller ;

    public RequestForwarder ( ILogger logger,
                              IMicroServiceCaller caller)
    {
        _logger      = logger ;
        _caller = caller ;
    }

    public async Task Forward ( HttpContext      httpContext )
    {
        var success = await _caller.TryCall ( httpContext ) ;

        _logger.Information ( $"Success: {success} - Forwarding '{httpContext.Request.GetDisplayUrl ( )}'" ) ;
    }
}

public interface IRequestForwarder
{
    Task Forward ( HttpContext httpContext ) ;
}