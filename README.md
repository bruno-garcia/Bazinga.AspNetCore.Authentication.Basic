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
    services.AddAuthorization()
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

And finally, since ASP.NET Core 2.0, the single middeware for authentication:

```csharp
public void Configure(IApplicationBuilder app)
{
    app.UseAuthentication();
    app.AddMvc();
}
```

For better understanding of the ASP.NET Core Identity, see [Microsoft docs](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity)
# License

Licensed under MIT
