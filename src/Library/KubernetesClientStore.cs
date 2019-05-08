using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Contrib.KubeClient.CustomResources;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.Extensions.Logging;

namespace Contrib.IdentityServer4.KubernetesStore
{
    [ExcludeFromCodeCoverage]
    public class KubernetesClientStore : InMemoryClientStore
    {
        public KubernetesClientStore(ICustomResourceWatcher<ClientResource> clientWatcher, ILogger<KubernetesClientStore> logger)
            : base(Filter(clientWatcher, logger))
        {}

        private static IEnumerable<Client> Filter(IEnumerable<ClientResource> resources, ILogger<KubernetesClientStore> logger)
        {
            foreach (var resource in resources)
            {
                var client = resource.Spec;

                if (string.IsNullOrEmpty(client.ClientId))
                    client.ClientId = resource.Metadata.Namespace + "-" + resource.Metadata.Name;

                if (client.AllowedGrantTypes.Count == 0)
                {
                    logger.LogWarning("Skipped client '{0}' because it does not specify a valid combination of allowed grant types.", client.ClientId);
                    continue;
                }

                yield return client;
            }
        }
    }
}
