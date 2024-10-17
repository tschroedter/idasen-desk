using System ;
using Serilog ;

namespace Idasen.SystemTray ;

class DefaultExceptionHandler : IExceptionHandler
{
    public bool CanHandle(Exception exception)
    {
        return true;
    }
    
    public void Handle(Exception exception, ILogger logger)
    {
        logger.Error(exception, exception.Message);
    }
}