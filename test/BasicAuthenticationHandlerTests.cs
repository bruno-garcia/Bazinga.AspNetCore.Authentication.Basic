using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Bazinga.AspNetCore.Authentication.Basic.Tests
{
    public class BasicAuthenticationHandlerTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("SomeRealm")]
        [InlineData("AnotherRealm")]
        public async Task NoAuthorizeOnRequestReturnsChallenge(string expectedRealm)
        {
            var client = TestBed.GetClient(o => o.Realm = expectedRealm);
            var response = await client.GetAsync("/");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Equal(string.Empty, await response.Content.ReadAsStringAsync());
            Assert.Equal($"Basic realm=\"{expectedRealm}\"", response.Headers.WwwAuthenticate.Single().ToString());
        }

        [Fact]
        public async Task DefaultTestBedRejectsCredentials()
        {
            var client = TestBed.GetClient();
            client.SetBasic("user", "pass");
            var response = await client.GetAsync("/");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Equal(string.Empty, await response.Content.ReadAsStringAsync());
            Assert.Equal("Basic realm=\"\"", response.Headers.WwwAuthenticate.Single().ToString());
        }

        [Fact]
        public async Task ValidCredentialsAuthorize()
        {
            const string username = "some-user";
            const string password = "password";
            var client = TestBed.GetClient(builder => builder.AddBasicAuthentication(
                userPass => Task.FromResult(userPass.username == username && userPass.password == password)));
            client.SetBasic(username, password);
            var response = await client.GetAsync("/");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(string.Empty, await response.Content.ReadAsStringAsync());
            Assert.False(response.Headers.WwwAuthenticate.Any());
        }
    }
}
