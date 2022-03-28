using Idasen.RESTAPI.MicroService.Shared.Interfaces ;
using JetBrains.Annotations ;
using Microsoft.AspNetCore.Http ;
using Microsoft.AspNetCore.Http.Extensions ;
using Serilog ;

namespace Idasen.RESTAPI.MicroService.Shared.RequestProcessing ;

[UsedImplicitly]
public class SettingsResponseCreator : ISettingsResponseCreator
{
    public async Task Response ( IServiceProvider serviceProvider ,
                                 HttpContext      httpContext )
    {
        var logger = serviceProvider.GetService ( typeof ( ILogger ) ) as ILogger ;
        var provider = serviceProvider.GetService ( typeof ( IMicroServiceSettingsProvider ) ) as IMicroServiceSettingsProvider ;
        var caller = serviceProvider.GetService ( typeof ( ISettingsCaller ) ) as ISettingsCaller ;

        if ( logger   == null ||
             caller   == null ||
             provider == null )
            return ;

        var success = await caller.TryCall ( httpContext ,
                                             provider ) ;

        logger.Information ( $"Success: {success} - Forwarding '{httpContext.Request.GetDisplayUrl ( )}'" ) ;
    }
}