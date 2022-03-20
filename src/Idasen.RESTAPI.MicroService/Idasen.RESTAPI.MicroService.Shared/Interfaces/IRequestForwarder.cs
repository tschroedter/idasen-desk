using Microsoft.AspNetCore.Http ;

namespace Idasen.RESTAPI.MicroService.Shared.Interfaces ;

public interface IRequestForwarder
{
    Task Forward ( HttpContext httpContext ) ;
}