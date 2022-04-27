using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(); // add MVC 

// add Authentication method (scheme)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.LoginPath = "/account/login";
    });

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
        // enable ClientCredentialsFlow
        options.AllowClientCredentialsFlow();

        // enable AuthorizationCodeFlow
        options
            .AllowAuthorizationCodeFlow()
            .RequireProofKeyForCodeExchange(); // enable PKCE

        options.AllowRefreshTokenFlow(); // for Refresh Token

        options
            .SetAuthorizationEndpointUris("/connect/authorize") // for AuthorizationCodeFlow
            .SetTokenEndpointUris("/connect/token")
            .SetUserinfoEndpointUris("/connect/userinfo"); // part of OpenID Connect (OIDC)

        // Encryption and signing of tokens (DEVELOPMENT ONLY!)
        options
            .AddEphemeralEncryptionKey()
            .AddEphemeralSigningKey()
            .DisableAccessTokenEncryption(); // disable token encryption

        // Register scopes (permissions)
        options.RegisterScopes("api");

        // Register the ASP.NET Core host and configure the ASP.NET Core-specific options.
        options
            .UseAspNetCore()
            .EnableTokenEndpointPassthrough()
            .EnableAuthorizationEndpointPassthrough() // for AuthorizationCodeFlow
            .EnableUserinfoEndpointPassthrough();  // part of OpenID Connect (OIDC)
    });

builder.Services.AddHostedService<TestData>(); // executes on app start

var app = builder.Build();

//  --------- start: config MVC ---------

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // activate Authentication

app.UseAuthorization(); // part of OpenID Connect (OIDC): Endpoint /connect/userinfo is secured

app.UseEndpoints(endpoints =>
{
    endpoints.MapDefaultControllerRoute();
});

//  --------- end: config  MVC ---------

//app.MapGet("/", () => "Hello World!");

app.Run();
