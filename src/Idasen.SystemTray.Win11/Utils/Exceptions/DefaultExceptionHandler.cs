using Serilog ;

namespace Idasen.SystemTray.Win11.Utils.Exceptions ;

public class DefaultExceptionHandler : IExceptionHandler
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