using Idasen.RESTAPI.Desk.Settings ;

namespace Idasen.RESTAPI.Desk.RequestProcessing ;

public interface ISettingsCaller
{
    Task < object? > TryCall ( HttpContext                   httpContext ,
                               IMicroServiceSettingsProvider provider ) ;
}