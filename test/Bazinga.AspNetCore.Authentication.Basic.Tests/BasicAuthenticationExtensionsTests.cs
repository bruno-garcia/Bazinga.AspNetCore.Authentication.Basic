using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Bazinga.AspNetCore.Authentication.Basic.Tests
{
    public class BasicAuthenticationExtensionsTests
    {
        [Fact]
        public Task AddBasicAuthentication_VerifyDefaultValuesOnFirstFuncOverload()
        {
            var services = new ServiceCollection();
            services.AddAuthentication().AddBasicAuthentication(a => Task.FromResult(false));
            return AssertConfiguration(services, true);
        }

        [Fact]
        public Task AddBasicAuthentication_VerifyDefaultValuesOnFirstGenericOverload()
        {
            var services = new ServiceCollection();
            services.AddAuthentication().AddBasicAuthentication<NegativeBasicCredentialVerifier>();
            return AssertConfiguration(services, false);
        }

        [Fact]
        public Task AddBasicAuthentication_VerifyDefaultValuesOnSecondFuncOverload()
        {
            var services = new ServiceCollection();
            services.AddAuthentication().AddBasicAuthentication(o => { }, a => Task.FromResult(false));
            return AssertConfiguration(services, true);
        }

        [Fact]
        public Task AddBasicAuthentication_VerifyDefaultValuesOnSecondGenericOverload()
        {
            var services = new ServiceCollection();
            services.AddAuthentication().AddBasicAuthentication<NegativeBasicCredentialVerifier>(o => { });
            return AssertConfiguration(services, false);
        }

        [Fact]
        public Task AddBasicAuthentication_VerifyAuthenticationSchemeOnGenericOverloadIsOverriden()
        {
            var services = new ServiceCollection();
            const string testScheme = "TestScheme";
            services.AddAuthentication().AddBasicAuthentication<NegativeBasicCredentialVerifier>(testScheme, o => { });
            return AssertConfiguration(services, false, expectedScheme: testScheme);
        }

        [Fact]
        public Task AddBasicAuthentication_VerifyAuthenticationSchemeOnFuncOverloadIsOverriden()
        {
            var services = new ServiceCollection();
            const string testScheme = "TestScheme";
            services.AddAuthentication().AddBasicAuthentication(testScheme, o => { }, a => Task.FromResult(false));
            return AssertConfiguration(services, true, expectedScheme: testScheme);
        }

        [Fact]
        public Task AddBasicAuthentication_VerifyAuthenticationSchemeAndDisplayNameOnGenericOverloadAreOverriden()
        {
            var services = new ServiceCollection();
            const string testScheme = "TestScheme";
            const string testDisplayName = "TestDisplayName";
            services.AddAuthentication().AddBasicAuthentication<NegativeBasicCredentialVerifier>(testScheme, testDisplayName, o => { });
            return AssertConfiguration(services, false, expectedScheme: testScheme, expectedDisplayName: testDisplayName);
        }

        [Fact]
        public Task AddBasicAuthentication_VerifyAuthenticationSchemeAndDisplayNameOnFuncOverloadAreOverriden()
        {
            var services = new ServiceCollection();
            const string testScheme = "TestScheme";
            const string testDisplayName = "TestDisplayName";
            services.AddAuthentication().AddBasicAuthentication(testScheme, testDisplayName, o => { }, a => Task.FromResult(false));
            return AssertConfiguration(services, true, expectedScheme: testScheme, expectedDisplayName: testDisplayName);
        }

        [Fact]
        public async Task AddBasicAuthentication_GenericOverloadDoesntOverrideRegistration()
        {
            var services = new ServiceCollection();
            services.AddSingleton<IBasicCredentialVerifier, NegativeBasicCredentialVerifier>();

            services.AddAuthentication().AddBasicAuthentication<FuncBasicCredentialVerifier>();
            var sp = await AssertConfiguration(services, true);

            Assert.IsType<NegativeBasicCredentialVerifier>(sp.GetRequiredService<IBasicCredentialVerifier>());
        }

        private async Task<IServiceProvider> AssertConfiguration(
            IServiceCollection services,
            bool verfierIsSingleton,
            string expectedScheme = BasicAuthenticationDefaults.AuthenticationScheme,
            string expectedDisplayName = null,
            Type handlerType = null)
        {
            expectedDisplayName = expectedDisplayName ?? BasicAuthenticationDefaults.DisplayName;
            handlerType = handlerType ?? typeof(BasicAuthenticationHandler);

            var sp = services.BuildServiceProvider();
            var verifier = sp.GetRequiredService<IBasicCredentialVerifier>();
            Assert.NotNull(verifier);
            Assert.Equal(verfierIsSingleton, sp.GetRequiredService<IBasicCredentialVerifier>() == sp.GetRequiredService<IBasicCredentialVerifier>());

            var schemeProvider = sp.GetRequiredService<IAuthenticationSchemeProvider>();
            var scheme = await schemeProvider.GetSchemeAsync(expectedScheme);
            Assert.NotNull(scheme);
            Assert.Same(handlerType, scheme.HandlerType);
            Assert.Same(expectedDisplayName, scheme.DisplayName);

            return sp;
        }
    }
}
