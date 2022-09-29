using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace api;

public class JwksRetrieverNew : IConfigurationRetriever<OpenIdConnectConfiguration>
{
    public async Task<OpenIdConnectConfiguration> GetConfigurationAsync(string address, IDocumentRetriever retriever, CancellationToken cancel)
    {
        if (string.IsNullOrWhiteSpace(address))
        {
            throw LogHelper.LogArgumentNullException(nameof(address));
        }

        if (retriever is null)
        {
            throw LogHelper.LogArgumentNullException(nameof(retriever));
        }

        var doc = await retriever.GetDocumentAsync(address, cancel).ConfigureAwait(false);
        var jwks = new JsonWebKeySet(doc);
        var openIdConnectConfiguration = new OpenIdConnectConfiguration()
        {
            JsonWebKeySet = jwks,
            JwksUri = address,
        };
        foreach (var securityKey in jwks.GetSigningKeys())
            openIdConnectConfiguration.SigningKeys.Add(securityKey);

        return openIdConnectConfiguration;
    }
}