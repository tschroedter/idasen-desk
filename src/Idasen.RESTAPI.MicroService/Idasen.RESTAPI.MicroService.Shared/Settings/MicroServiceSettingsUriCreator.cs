using Serilog ;

namespace Idasen.RESTAPI.MicroService.Shared.Settings ;

public class MicroServiceSettingsUriCreator : IMicroServiceSettingsUriCreator
{
    private readonly ILogger _logger ;

    public MicroServiceSettingsUriCreator ( ILogger logger)
    {
        _logger = logger ;
    }

    public Uri GetUri ( MicroServiceSettings settings )
    {
        _logger.Information ( $"{nameof(MicroServiceSettings)}: {settings}" );

        var uriString = settings.Protocol +
                        "://"             +
                        settings.Host     +
                        ":"               +
                        settings.Port     +
                        settings.Path;

        var uri = new Uri(uriString);

        _logger.Information($"Create Uri: {uri}");

        return uri ;
    }
}