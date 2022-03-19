using Idasen.RESTAPI.Desk.Settings ;
using Microsoft.AspNetCore.Http.Extensions ;
using ILogger = Serilog.ILogger ;

namespace Idasen.RESTAPI.Desk.RequestProcessing ;

public class SettingsResponseCreator
{
    public async Task Response ( IServiceProvider serviceProvider ,
                                 HttpContext      httpContext )
    {
        var logger   = serviceProvider.GetService < ILogger > ( ) ;
        var provider = serviceProvider.GetService < IMicroServiceSettingsProvider > ( ) ;
        var caller   = serviceProvider.GetService < ISettingsCaller > ( ) ;

        if ( logger   == null ||
             caller   == null ||
             provider == null )
            return ;

        var success = await caller.TryCall ( httpContext ,
                                             provider ) ;

        logger.Information ( $"Success: {success} - Forwarding '{httpContext.Request.GetDisplayUrl ( )}'" ) ;
    }
}