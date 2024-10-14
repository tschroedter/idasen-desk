using Serilog;

namespace Idasen.SystemTray.Win11.Interfaces;

interface IExceptionHandler
{
    bool CanHandle(Exception exception);

    void Handle(Exception exception, ILogger logger);
}