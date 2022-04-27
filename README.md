# Setting up an Authorization Server with OpenIddict

Source: <https://dev.to/robinvanderknaap/setting-up-an-authorization-server-with-openiddict-part-i-introduction-4jid>

## Setup ASP.NET project

Part 2: <https://dev.to/robinvanderknaap/setting-up-an-authorization-server-with-openiddict-part-ii-create-aspnet-project-4949>

### Setup MVC

<https://dev.to/robinvanderknaap/setting-up-an-authorization-server-with-openiddict-part-ii-create-aspnet-project-4949>
```
dotnet new web --name AuthorizationServer
```
```
dotnet dev-certs https --trust
```

### Add Cookie Authentication

Use cookie authentication without ASP.NET Core Identity:
<https://docs.microsoft.com/en-us/aspnet/core/security/authentication/cookie?view=aspnetcore-6.0>

Login/Logout buttons are working now.

## Implement 'Client Credentials Flow'

Part 3: <https://dev.to/robinvanderknaap/setting-up-an-authorization-server-with-openiddict-part-iii-client-credentials-flow-55lp>

```
dotnet add package OpenIddict
dotnet add package OpenIddict.AspNetCore
dotnet add package OpenIddict.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.InMemory
```


```cs
builder.Services.AddDbContext<DbContext>(options =>
{
    // Configure the context to use an in-memory store.
    options.UseInMemoryDatabase(nameof(DbContext));

    // Register the entity sets needed by OpenIddict.
    options.UseOpenIddict();
});

builder.Services.AddOpenIddict()

    // Register the OpenIddict core components.
    .AddCore(options =>
    {
        // Configure OpenIddict to use the EF Core stores/models.
        options
            .UseEntityFrameworkCore()
            .UseDbContext<DbContext>();
    })

    // Register the OpenIddict server components.
    .AddServer(options =>
    {
        options.AllowClientCredentialsFlow();

        options.SetTokenEndpointUris("/connect/token");

        // Encryption and signing of tokens (DEVELOPMENT ONLY!)
        options
            .AddEphemeralEncryptionKey()
            .AddEphemeralSigningKey();

        // Register scopes (permissions)
        options.RegisterScopes("api");

        // Register the ASP.NET Core host and configure the ASP.NET Core-specific options.
        options
            .UseAspNetCore()
            .EnableTokenEndpointPassthrough();
    });
```

Check implementation: https://localhost:5001/.well-known/openid-configuration

Request-Headers according <https://datatracker.ietf.org/doc/html/rfc6749#section-4.1.3>


Get access token, using Authentication tab in Postman:
<https://learning.postman.com/docs/sending-requests/authorization/#oauth-20>

Manually Post in Postman to get Access Token:

```bash
curl --location --request POST 'https://localhost:5001/connect/token' \
--header 'Content-Type: application/x-www-form-urlencoded' \
--data-urlencode 'grant_type=client_credentials' \
--data-urlencode 'client_id=postman' \
--data-urlencode 'client_secret=postman-secret'
```

Result:

```json
{
  "access_token": "eyJhbGciOiJSU0EtT0FF...",
  "token_type": "Bearer",
  "expires_in": 3599
}
```

Disable token encryption

```cs
options
    .AddEphemeralEncryptionKey()
    .AddEphemeralSigningKey()
    .DisableAccessTokenEncryption(); // disable token encryption
```

Check token in <https://jwt.io/>
