# Setting up an Authorization Server with OpenIddict

Source: https://dev.to/robinvanderknaap/setting-up-an-authorization-server-with-openiddict-part-i-introduction-4jid

## Setup ASP.NET project
### Setup MVC

<https://dev.to/robinvanderknaap/setting-up-an-authorization-server-with-openiddict-part-ii-create-aspnet-project-4949>
```
dotnet new web --name AuthorizationServer
```
```
dotnet dev-certs https --trust
```

## Add Cookie Authentication

Use cookie authentication without ASP.NET Core Identity:
<https://docs.microsoft.com/en-us/aspnet/core/security/authentication/cookie?view=aspnetcore-6.0>
