using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace Bazinga.AspNetCore.Authentication.Basic.Tests
{
    internal static class TestBed
    {
        public static void SetBasic(this HttpClient client, string username, string password)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}")));
        }

        public static HttpClient GetClient()
        {
            return GetClient((BasicAuthenticationOptions options) => { });
        }
        public static HttpClient GetClient(Action<AuthenticationBuilder> builderAction)
        {
            return CreateServer(builderAction).CreateClient();
        }

        public static HttpClient GetClient(Action<BasicAuthenticationOptions> options)
        {
            return CreateServer(b => b.AddBasicAuthentication<NegativeBasicCredentialVerifier>(options)).CreateClient();
        }

        public static TestServer CreateServer(Action<AuthenticationBuilder> builderAction)
        {
            var builder = new WebHostBuilder()
                .Configure(app =>
                {
                    app.UseAuthentication();
                    app.Use(async (context, next) =>
                    {
                        if (context.Request.Path == new PathString("/"))
                        {
                            var result = await context.AuthenticateAsync(BasicAuthenticationDefaults.AuthenticationScheme);
                            if (!result.Succeeded)
                                await context.ChallengeAsync(BasicAuthenticationDefaults.AuthenticationScheme);
                        }
                        else
                        {
                            await next();
                        }
                    });
                })
                .ConfigureServices(services =>
                {
                    builderAction(services.AddAuthentication(BasicAuthenticationDefaults.AuthenticationScheme));
                });

            return new TestServer(builder);
        }
    }
}
