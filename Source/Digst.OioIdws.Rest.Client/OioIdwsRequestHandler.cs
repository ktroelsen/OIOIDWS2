﻿using System;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Digst.OioIdws.Rest.Common;

namespace Digst.OioIdws.Rest.Client
{
    /// <summary>
    /// Can be used by HttpClient or similar to handle plumbing of issuing tokens and token expirations.
    /// The handler is not thread-safe.
    /// Renews tokens one time if access is denied.
    /// Uses <see cref="OioIdwsClient"/> to retrieve STS and access tokens.
    /// </summary>
    public class OioIdwsRequestHandler : WebRequestHandler
    {
        private readonly OioIdwsClient _client;
        private AccessToken.AccessToken _accessToken;

        /// <summary>
        /// Constructor that always uses a client certificate if one exist for TLS client authentication.
        /// </summary>
        /// <param name="client">Backpointer to OioIdwsClient</param>
        /// <param name="accessToken">An optional access token. Can be used if client already has access to a cached token.</param>
        public OioIdwsRequestHandler(OioIdwsClient client, AccessToken.AccessToken accessToken)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            _client = client;
            _accessToken = accessToken;

            //We can't know in advance whether it's a Bearer/Holder-of-key token we're going to work with. Either way we just add the certificate to the request, if given
            if (client.Settings.ClientCertificate != null)
            {
                ClientCertificates.Add(client.Settings.ClientCertificate);
            }
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await SendAuthenticatedRequest(request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized && IsInvalidToken(response))
            {
                //if the request is denied due to the token no longer being valid, we flush the tokens, ensure they are reloaded, and refire the request
                _accessToken = null;
                response = await SendAuthenticatedRequest(request, cancellationToken);
            }

            return response;
        }

        private async Task<HttpResponseMessage> SendAuthenticatedRequest(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            await EnsureValidAccessTokenAsync(cancellationToken);
            request.Headers.Authorization = new AuthenticationHeaderValue(AccessTokenTypeParser.ToString(_accessToken.Type), _accessToken.Value);
            return await base.SendAsync(request, cancellationToken);
        }

        private static bool IsInvalidToken(HttpResponseMessage response)
        {
            return response.Headers.WwwAuthenticate.Any(x =>
                AccessTokenTypeParser.FromString(x.Scheme).HasValue &&
                HttpHeaderUtils.ParseOAuthSchemeParameter(x.Parameter)["error"].Equals(AuthenticationErrorCodes.InvalidToken, StringComparison.OrdinalIgnoreCase));
        }

        private async Task EnsureValidAccessTokenAsync(CancellationToken cancellationToken)
        {
            if (_accessToken == null || !_accessToken.IsValid())
            {
                var securityToken = _client.GetSecurityToken();
                _accessToken = await _client.GetAccessTokenAsync(securityToken, cancellationToken);
            }
        }
    }
}
