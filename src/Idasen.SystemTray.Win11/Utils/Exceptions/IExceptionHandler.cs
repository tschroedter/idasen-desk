using Serilog ;

namespace Idasen.SystemTray.Win11.Utils.Exceptions ;

interface IExceptionHandler
{
    bool CanHandle(Exception exception);

    void Handle(Exception exception, ILogger logger);
}