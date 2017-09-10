using System;
using System.Threading.Tasks;
using Bazinga.AspNetCore.Authentication.Basic;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class BasicAuthenticationExtensions
    {
        public static AuthenticationBuilder AddBasicAuthentication(this AuthenticationBuilder builder, Func<(string username, string password), Task<bool>> verify)
            => builder.AddBasicAuthentication(BasicAuthenticationDefaults.AuthenticationScheme, _ => { }, verify: verify);

        public static AuthenticationBuilder AddBasicAuthentication(this AuthenticationBuilder builder, Action<BasicAuthenticationOptions> configureOptions, Func<(string username, string password), Task<bool>> verify)
            => builder.AddBasicAuthentication(BasicAuthenticationDefaults.AuthenticationScheme, configureOptions, verify: verify);

        public static AuthenticationBuilder AddBasicAuthentication(this AuthenticationBuilder builder, string authenticationScheme, Action<BasicAuthenticationOptions> configureOptions, Func<(string username, string password), Task<bool>> verify)
            => builder.AddBasicAuthentication(authenticationScheme, displayName: BasicAuthenticationDefaults.DisplayName, configureOptions: configureOptions, verify: verify);

        public static AuthenticationBuilder AddBasicAuthentication(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<BasicAuthenticationOptions> configureOptions, Func<(string username, string password), Task<bool>> verify)
        {
            builder.Services.AddSingleton<IBasicCredentialVerifier>(p => new FuncBasicCredentialVerifier(verify));
            return builder.AddBasicAuthentication(authenticationScheme, displayName, configureOptions);
        }

        public static AuthenticationBuilder AddBasicAuthentication<TVerifier>(this AuthenticationBuilder builder)
            where TVerifier : IBasicCredentialVerifier
            => builder.AddBasicAuthentication<TVerifier>(BasicAuthenticationDefaults.AuthenticationScheme, _ => { });

        public static AuthenticationBuilder AddBasicAuthentication<TVerifier>(this AuthenticationBuilder builder, Action<BasicAuthenticationOptions> configureOptions)
            where TVerifier : IBasicCredentialVerifier
            => builder.AddBasicAuthentication<TVerifier>(BasicAuthenticationDefaults.AuthenticationScheme, configureOptions);

        public static AuthenticationBuilder AddBasicAuthentication<TVerifier>(this AuthenticationBuilder builder, string authenticationScheme, Action<BasicAuthenticationOptions> configureOptions)
            where TVerifier : IBasicCredentialVerifier
            => builder.AddBasicAuthentication<TVerifier>(authenticationScheme, displayName: BasicAuthenticationDefaults.DisplayName, configureOptions: configureOptions);

        public static AuthenticationBuilder AddBasicAuthentication<TVerifier>(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<BasicAuthenticationOptions> configureOptions)
            where TVerifier : IBasicCredentialVerifier
            => builder.AddBasicAuthentication<TVerifier>(authenticationScheme, displayName, configureOptions, ServiceLifetime.Transient);

        public static AuthenticationBuilder AddBasicAuthentication<TVerifier>(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<BasicAuthenticationOptions> configureOptions, ServiceLifetime verifierLifetime)
            where TVerifier : IBasicCredentialVerifier
        {
            builder.Services.TryAdd(new ServiceDescriptor(typeof(IBasicCredentialVerifier), typeof(TVerifier), verifierLifetime));
            return builder.AddBasicAuthentication(authenticationScheme, displayName, configureOptions);
        }

        /// <see cref="BasicAuthenticationHandler"/> expects IBasicCredentialVerifier to be registered
        private static AuthenticationBuilder AddBasicAuthentication(
            this AuthenticationBuilder builder,
            string authenticationScheme,
            string displayName,
            Action<BasicAuthenticationOptions> configureOptions)
        {
            return builder.AddScheme<BasicAuthenticationOptions, BasicAuthenticationHandler>(authenticationScheme, displayName, configureOptions);
        }
    }
}