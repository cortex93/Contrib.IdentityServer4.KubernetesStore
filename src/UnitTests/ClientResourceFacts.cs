using FluentAssertions;
using IdentityServer4.Models;
using Newtonsoft.Json;
using Xunit;

namespace Contrib.IdentityServer4.KubernetesStore
{
    public class ClientResourceFacts
    {
        [Fact]
        public void AcceptsValidGrantTypeCombinations()
        {
            var resource = Deserialize("{\"spec\": {\"clientName\": \"My App\", \"allowedGrantTypes\": [\"authorization_code\"]}}");
            resource.Spec.Should().BeEquivalentTo(new Client {ClientName = "My App", AllowedGrantTypes = {"authorization_code"}});
        }

        [Fact]
        public void RejectsInvalidGrantTypeCombinationsWithoutException()
        {
            var resource = Deserialize("{\"spec\": {\"clientName\": \"My App\", \"allowedGrantTypes\": [\"authorization_code\", \"hybrid\"]}}");
            resource.SerializationErrors.Should().HaveCount(1);
        }

        private static ClientResource Deserialize(string json)
            => JsonConvert.DeserializeObject<ClientResource>(json, ClientResource.Definition.SerializerSettings);
    }
}
