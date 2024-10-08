using Microsoft.Extensions.Configuration ;

namespace Idasen.SystemTray.Win11 ;

public interface IIdasenConfigurationProvider
{
    IConfigurationRoot GetConfiguration() ;
}