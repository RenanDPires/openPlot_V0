using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using openPlot.BuildingBlocks.Options;

namespace openPlot.BuildingBlocks.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAppOptions(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<SessionOptionsEx>(config.GetSection("Session"));
        services.Configure<AuthOptions>(config.GetSection("Auth"));
        return services;
    }
}
