# OAuthJwtAssertionTokenClient #

## Description ##
Client helper for OAuth 2.0 "Client Credentials" flow with JWT token assertion.

## Features ##
- Client Credentials Grant (2-Legged OAuth)
- "private_key_jwt" authentication method as defined in [OpenID Connect Core 1.0](http://openid.net/specs/openid-connect-core-1_0.html#ClientAuthentication)
- **Access token caching**. Uses in-memory cache by default. Caching key includes all parameters, so it is safe to use with more than one Authorization Server, credential set or OAuth scope list.

_If you need support for other grant types or authentication methods, please check [IdentityModel](https://github.com/IdentityModel/IdentityModel)._

## Getting started ##
Install from NuGet:
```powershell
Install-Package OAuthJwtAssertionTokenClient
```

Obtaining access token from Authorization Server token endpoint is as simple as this:

```csharp
var tokenClient = new JwtAssertionTokenClient(
                        authorizationServerUrl,
                        clientId,
                        x509certificate,
                        new[] { scope1, scope2 }
                  );

var accessToken = await tokenClient.GetToken();
```
