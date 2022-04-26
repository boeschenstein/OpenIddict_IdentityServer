using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(); // add MVC 

// add Authentication method (scheme)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.LoginPath = "/account/login";
    });

var app = builder.Build();

//  --------- start: config MVC ---------

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // activate Authentication

app.UseEndpoints(endpoints =>
{
    endpoints.MapDefaultControllerRoute();
});

//  --------- end: config  MVC ---------

//app.MapGet("/", () => "Hello World!");

app.Run();
