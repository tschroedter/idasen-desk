using Microsoft.AspNetCore.Http ;

namespace Idasen.RESTAPI.MicroService.Shared.Interfaces ;

public interface ISettingsCaller
{
    Task < object? > TryCall ( HttpContext                   httpContext ,
                               IMicroServiceSettingsProvider provider ) ;
}