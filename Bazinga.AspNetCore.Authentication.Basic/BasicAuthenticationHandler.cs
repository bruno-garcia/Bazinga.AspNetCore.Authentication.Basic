using System;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace Bazinga.AspNetCore.Authentication.Basic
{
    internal class BasicAuthenticationHandler : AuthenticationHandler<BasicAuthenticationOptions>
    {
        private readonly IBasicCredentialVerifier _authenticationVerifier;

        protected override Task<object> CreateEventsAsync() => Task.FromResult<object>(new BasicAuthenticationEvents());

        protected new BasicAuthenticationEvents Events
        {
            get => (BasicAuthenticationEvents)base.Events;
            set => base.Events = value;
        }

        public BasicAuthenticationHandler(
            IBasicCredentialVerifier authenticationVerifier,
            IOptionsMonitor<BasicAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock) 
            => _authenticationVerifier = authenticationVerifier ?? throw new ArgumentNullException(nameof(authenticationVerifier));

        // https://tools.ietf.org/html/rfc2617#section-2
        private static (string userid, string password) DecodeUserIdAndPassword(string encodedAuth)
        {
            var userpass = Encoding.UTF8.GetString(Convert.FromBase64String(encodedAuth));

            var separator = userpass.IndexOf(':');
            if (separator == -1) throw new InvalidOperationException("Invalid Authorization header: Missing separator character ':'. See RFC2617.");

            return (userpass.Substring(0, separator), userpass.Substring(separator + 1));
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            try
            {
                string auth = Request.Headers["Authorization"];
                if (string.IsNullOrEmpty(auth))
                {
                    return AuthenticateResult.NoResult();
                }

                string encodedAuth = null;
                if (auth.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
                {
                    encodedAuth = auth.Substring("Basic ".Length).Trim();
                }

                if (string.IsNullOrEmpty(encodedAuth))
                {
                    return AuthenticateResult.NoResult();
                }

                var userpass = DecodeUserIdAndPassword(encodedAuth);

                if (!await _authenticationVerifier.Authenticate(userpass.userid, userpass.password))
                {
                    Logger.LogInformation("Failed to validate {userid}.", userpass.userid);
                    return AuthenticateResult.Fail("Failed to validate userid/password.");
                }

                Logger.LogInformation("Successfully validated credentials for {userid}.", userpass.userid);

                var claims = new[] { new Claim(ClaimTypes.Name, userpass.userid, ClaimValueTypes.String, Options.ClaimsIssuer) };
                var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, Scheme.Name));

                var successContext = new AuthenticationSucceededContext(userpass.userid, Context, Scheme, Options)
                {
                    Principal = principal
                };

                await Events.CredentialsValidated(successContext);
                if (successContext.Result != null)
                {
                    return successContext.Result;
                }

                successContext.Success();
                return successContext.Result;
            }
            catch (Exception ex)
            {
                Logger.LogError("Exception occurred while processing message.", ex);

                var authenticationFailedContext = new AuthenticationFailedContext(Context, Scheme, Options)
                {
                    Exception = ex
                };

                await Events.AuthenticationFailed(authenticationFailedContext);
                if (authenticationFailedContext.Result != null)
                {
                    return authenticationFailedContext.Result;
                }

                throw;
            }
        }

        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            var authResult = await HandleAuthenticateOnceSafeAsync();
            var eventContext = new BasicAuthenticationChallengeContext(Context, Scheme, Options, properties)
            {
                AuthenticateFailure = authResult?.Failure
            };

            await Events.Challenge(eventContext);
            if (eventContext.Handled)
            {
                return;
            }

            Response.StatusCode = 401;
            Response.Headers.Append(HeaderNames.WWWAuthenticate, Options.Challenge);
        }
    }
}