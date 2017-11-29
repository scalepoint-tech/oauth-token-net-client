# OAuth Token Endpoint Client for .NET #

## Description ##
Client helper for OAuth2 Token endpoint. Supports "Client Credentials" flow with "client_secret", RS256 JWT "client_assertion", custom grants and token caching.

## Features ##
- Client Credentials Grant (2-Legged OAuth)
- "client_secret" authentication
- "private_key_jwt" authentication method as defined in [Assertion Framework for OAuth 2.0 Client Authentication and Authorization Grants](https://tools.ietf.org/html/rfc7521#section-6.2) and [OpenID Connect Core 1.0](http://openid.net/specs/openid-connect-core-1_0.html#ClientAuthentication)
- Custom grant support. I.e. for custom assertion grants.
- **Access token caching**. Uses in-memory cache by default. Caching key includes all parameters, so it is safe to use with more than one Authorization Server, credential set or OAuth scope list.

_If you need support for other grant types or authentication methods, please check [IdentityModel](https://github.com/IdentityModel/IdentityModel)._

## Getting started ##
Install from NuGet:
```powershell
Install-Package Scalepoint.OAuth.TokenClient
```

Obtaining access token from Authorization Server token endpoint is as simple as this:

###### private_key_jwt ######

```csharp
var tokenClient = new ClientCredentialsGrantTokenClient(
                        tokenEndpointUrl,
                        new JwtBearerClientAssertionCredentials(
                            tokenEndpointUrl,
                            clientId,
                            x509certificate
                        )
                  );

var accessToken = await tokenClient.GetTokenAsync(new [] { scope1, scope2 });
```

###### client_secret ######

```csharp
var tokenClient = new ClientCredentialsGrantTokenClient(
                        tokenEndpointUrl,
                        new ClientSecretCredentials(
                            clientId,
                            clientSecret
                        )
                  );

var accessToken = await tokenClient.GetTokenAsync(new [] { scope1, scope2 });
```
