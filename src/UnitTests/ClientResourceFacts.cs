using FluentAssertions;
using IdentityServer4.Models;
using Newtonsoft.Json;
using Xunit;

namespace Contrib.IdentityServer4.KubernetesStore
{
    public class ClientResourceFacts
    {
        [Fact]
        public void AcceptsValidGrantTypeCombinationsException()
        {
            VerifyDeserialize(
                "{\"clientName\": \"My App\", \"allowedGrantTypes\": [\"authorization_code\"]}",
                new Client {ClientName = "My App", AllowedGrantTypes = {"authorization_code"}});
        }

        [Fact]
        public void RejectsInvalidGrantTypeCombinationsWithoutException()
        {
            VerifyDeserialize(
                "{\"clientName\": \"My App\", \"allowedGrantTypes\": [\"authorization_code\", \"hybrid\"]}",
                new Client {ClientName = "My App"});
        }

        private static void VerifyDeserialize(string json, Client expectation)
            => JsonConvert.DeserializeObject<Client>(json, ClientResource.Definition.SerializerSettings)
                          .Should().BeEquivalentTo(expectation);
    }
}
