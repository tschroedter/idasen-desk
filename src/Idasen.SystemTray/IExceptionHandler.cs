using System ;
using Serilog ;

namespace Idasen.SystemTray ;

interface IExceptionHandler
{
    bool CanHandle(Exception exception);
    
    void Handle(Exception    exception, ILogger logger);
}