using Microsoft.Extensions.Configuration ;

namespace Idasen.SystemTray.Win11.Interfaces ;

public interface IIdasenConfigurationProvider
{
    IConfigurationRoot GetConfiguration ( ) ;
}