# Bazinga.AspNetCore.Authentication.Basic 
[![Build status](https://ci.appveyor.com/api/projects/status/hnl0ixy7oa7mrq7x/branch/master?svg=true)](https://ci.appveyor.com/project/bruno-garcia/bazinga-aspnetcore-authentication-basic/branch/master) [![NuGet](https://img.shields.io/nuget/v/Bazinga.AspNetCore.Authentication.Basic.svg)](https://www.nuget.org/packages/Bazinga.AspNetCore.Authentication.Basic/)
[![codecov](https://codecov.io/gh/bruno-garcia/Bazinga.AspNetCore.Authentication.Basic/branch/master/graph/badge.svg)](https://codecov.io/gh/bruno-garcia/Bazinga.AspNetCore.Authentication.Basic)
Basic Authentication for Microsoft ASP.NET Core Security

Microsoft doesn't ship a Basic Authentication package with ASP.NET Core Security for a good reason.
While that doesn't stop us needing such implementation for testing, this is not advised for production systems due to the many pitfalls and insecurities.

Sample usages, with hard-coded credentials:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddMvc();
    services.AddAuthentication(BasicAuthenticationDefaults.AuthenticationScheme)
        .AddBasicAuthentication(credentials => 
            Task.FromResult(
                credentials.username == "myUsername" 
                && credentials.password == "myPassword"));
}
```

Or by defining a service to register. Allowing your validator to take dependencies through Dependency Injection:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddAuthentication(BasicAuthenticationDefaults.AuthenticationScheme)
        .AddBasicAuthentication<DatabaseBasicCredentialVerifier>();
}

// With your own validator
public class DatabaseBasicCredentialVerifier : IBasicCredentialVerifier
{
    private readonly IUserRepository _db;
    
    public DatabaseBasicCredentialVerifier(IUserRepository db) => _db = db;

    public Task<bool> Authenticate(string username, string password)
    {
        return _db.IsValidAsync(username, password);
    }
}
```

And finally, middeware for authentication:

```csharp
public void Configure(IApplicationBuilder app)
{
    app.UseAuthentication(); 
    app.UseAuthorization();
    app.UseMvc();
}
```

If you need to include your own claim types, after the user is authenticated, an event called OnCredentialsValidated is dispatched, see the code below:

```csharp
public void ConfigureServices()
{
    services.AddAuthentication(BasicAuthenticationDefaults.AuthenticationScheme)
        .AddBasicAuthentication<BasicAuthenticationVerifier>(options => {
            options.Events = new BasicAuthenticationEvents()
            {
                OnCredentialsValidated = (context) =>
                {
                    context.Principal = new ClaimsPrincipal(); // New instance of claims principal with your claims
                    return Task.FromResult(context);
                }
            };
        });

}
```

If inside the function you need call a service from DI, you can use:

```csharp
public void ConfigureServices()
{
    services.AddAuthentication(BasicAuthenticationDefaults.AuthenticationScheme)
        .AddBasicAuthentication<BasicAuthenticationVerifier>(options => {
            options.Events = new BasicAuthenticationEvents()
            {
                OnCredentialsValidated = (context) =>
                {
                    var authService = context.HttpContext.RequestServices.GetRequiredService<AuthenticationService>();

                    context.Principal = authService.GetClaimPrincipal(); // New instance of claims principal with your claims
                    return Task.FromResult(context);
                }
            };
        });

}
```

Regardless the configuration above you choose to use, you need to ensure your controller expects an authenticated user. 
That can be acomplished in different ways, one being the: `[Authorize]` attribute. [See Microsoft docs](https://docs.microsoft.com/en-us/aspnet/core/security/authorization/simple?view=aspnetcore-2.1) for more.

For better understanding of the ASP.NET Core Identity, see [Microsoft docs](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity)
# License

Licensed under MIT
