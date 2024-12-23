using Yarp.ReverseProxy.Configuration;

namespace Bff;

internal static class YarpConfiguration
{
    internal static (List<RouteConfig>, List<ClusterConfig>) GetYarpConfigurations(ProxySettings proxySettings)
    {
        var writeDestination = new DestinationConfig
        {
            Address = proxySettings.WriteApiAddress.ToString()
        };
        
        var writeDestinations = new Dictionary<string, DestinationConfig>
            { { "WriteCluster/Destination", writeDestination } };
        
        var writeClusterConfig = new ClusterConfig
        {
            ClusterId = "WriteCluster",
            Destinations = writeDestinations
        };
        
        var readDestination = new DestinationConfig
        {
            Address = proxySettings.ReadApiAddress.ToString()
        };
        
        var readDestinations = new Dictionary<string, DestinationConfig>
            { { "ReadCluster/Destination", readDestination } };
        
        var readClusterConfig = new ClusterConfig
        {
            ClusterId = "ReadCluster",
            Destinations = readDestinations
        };
        
        List<ClusterConfig> clusters =
        [
            writeClusterConfig,
            readClusterConfig
        ];

        List<RouteConfig> routes =
        [
            new()
            {
                RouteId = "WriteRoute",
                ClusterId = writeClusterConfig.ClusterId,
                Match = new RouteMatch
                {
                    Methods = [HttpMethod.Post.Method, HttpMethod.Put.Method, HttpMethod.Delete.Method],
                    Path = "/{**catchall}"
                }
            },
            new()
            {
                RouteId = "ReadRoute",
                ClusterId = readClusterConfig.ClusterId,
                Match = new RouteMatch
                {
                    Methods = [HttpMethod.Get.Method],
                    Path = "/{**catchall}"
                }
            }
        ];

        return (routes, clusters);
    }
}