namespace Idasen.RESTAPI.Desk.RequestProcessing ;

public interface IMicroServiceCaller
{
    Task < bool > TryCall ( HttpContext                   httpContext ) ;
}