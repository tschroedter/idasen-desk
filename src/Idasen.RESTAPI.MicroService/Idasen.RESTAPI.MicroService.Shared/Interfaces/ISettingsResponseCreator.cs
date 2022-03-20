using Microsoft.AspNetCore.Http ;

namespace Idasen.RESTAPI.MicroService.Shared.Interfaces ;

public interface ISettingsResponseCreator
{
    Task Response ( IServiceProvider serviceProvider ,
                    HttpContext      httpContext ) ;
}