using Microsoft.AspNetCore.Http ;

namespace Idasen.RESTAPI.MicroService.Shared.Interfaces ;

public interface IMicroServiceCaller
{
    Task < bool > TryCall ( HttpContext httpContext ) ;
}