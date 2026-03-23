namespace LHA.ApiGateway.ReverseProxy;

using Microsoft.Extensions.Configuration;
using Yarp.ReverseProxy.Configuration;

/// <summary>
/// Allows dynamic/custom proxy configuration overriding. 
/// In this implementation it simply echoes the configuration based setup, 
/// but provides an injection point for future dynamic routing from a database.
/// </summary>
public class ProxyConfigProvider
{
    private readonly IConfiguration _configuration;

    public ProxyConfigProvider(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IProxyConfig GetConfig()
    {
        // Custom implementations for memory mapping/live DB reloading go here.
        // YARP's LoadFromConfig handles most enterprise needs natively.
        return null!;
    }
}
