using System;
using System.Linq;
using System.Threading.Tasks;
using Contrib.KubeClient.CustomResources;
using FluentAssertions;
using IdentityServer4.Models;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Contrib.IdentityServer4.KubernetesStore
{
    public class KubernetesClientStoreFacts
    {
        private readonly Mock<ILogger<KubernetesClientStore>> _loggerMock = new Mock<ILogger<KubernetesClientStore>>();

        [Fact]
        public async Task GetsClientById()
        {
            var store = GetStore(
                new ClientResource("mynamespace", "myname", new Client
                {
                    ClientId = "myid",
                    ClientName = "My Name",
                    AllowedGrantTypes = {"implicit"}
                }));

            var client = await store.FindClientByIdAsync("myid");
            client.Should().BeEquivalentTo(new Client
            {
                ClientId = "myid",
                ClientName = "My Name",
                AllowedGrantTypes = {"implicit"}
            });
        }

        [Fact]
        public async Task AutoGeneratesMissingClientId()
        {
            var store = GetStore(
                new ClientResource("mynamespace", "myname", new Client
                {
                    ClientName = "My Name",
                    AllowedGrantTypes = {"implicit"}
                }));

            var client = await store.FindClientByIdAsync("mynamespace-myname");
            client.Should().BeEquivalentTo(new Client
            {
                ClientId = "mynamespace-myname",
                ClientName = "My Name",
                AllowedGrantTypes = {"implicit"}
            });
        }

        [Fact]
        public async Task SkipsAndLogsClientsWithoutAllowedGrantTypes()
        {
            var store = GetStore(
                new ClientResource("mynamespace", "myname", new Client
                {
                    ClientId = "myid",
                    ClientName = "My Name"
                }));

            var client = await store.FindClientByIdAsync("myid");
            client.Should().BeNull();
            _loggerMock.Verify(x => x.Log(LogLevel.Warning, 0, It.IsAny<object>(), null, It.IsAny<Func<object, Exception, string>>()));
        }

        private KubernetesClientStore GetStore(params ClientResource[] resources)
        {
            var watcherMock = new Mock<ICustomResourceWatcher<ClientResource>>();
            watcherMock.Setup(x => x.GetEnumerator())
                       .Returns(() => resources.ToList().GetEnumerator());

            return new KubernetesClientStore(watcherMock.Object, _loggerMock.Object);
        }
    }
}
