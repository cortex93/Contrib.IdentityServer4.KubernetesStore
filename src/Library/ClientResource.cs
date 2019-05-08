using System.Collections.Generic;
using Contrib.KubeClient.CustomResources;
using IdentityServer4.Models;
using KellermanSoftware.CompareNetObjects;
using Microsoft.AspNetCore.JsonPatch;
using Newtonsoft.Json.Serialization;

namespace Contrib.IdentityServer4.KubernetesStore
{
    public class ClientResource : CustomResource<Client>, IPayloadPatchable<ClientResource>
    {
        public new static CustomResourceDefinition Definition { get; }
            = new CustomResourceDefinition("contrib.identityserver.io/v1", "oauthclients", "OauthClient")
            {
                SerializerSettings =
                {
                    Converters = {new ClaimConverter()},
                    Error = ErrorHandler
                }
            };

        private static void ErrorHandler(object sender, ErrorEventArgs e)
        {
            // Invalid combinations of grant types cannot be prevented by K8s using OpenAPI Spec.
            // Avoid throwing exceptions during deserialization. Handled in KubernetesClientStore instead.
            if (e.ErrorContext.Path.Contains("allowedGrantTypes"))
            {
                e.ErrorContext.Handled = true;
                (e.ErrorContext.OriginalObject as ICollection<string>)?.Clear();
            }
        }

        public ClientResource()
            : base(Definition)
        {}

        public ClientResource(string @namespace, string name, Client spec)
            : base(Definition, @namespace, name, spec)
        {}

        private static readonly CompareLogic SpecComparer = new CompareLogic();

        protected override bool SpecEquals(Client other)
            => SpecComparer.Compare(Spec, other).AreEqual;

        public void ToPayloadPatch(JsonPatchDocument<ClientResource> patch)
            => patch.Replace(x => x.Spec, Spec);
    }
}
