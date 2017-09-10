using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace Bazinga.AspNetCore.Authentication.Basic
{
    public class BasicAuthenticationChallengeContext : PropertiesContext<BasicAuthenticationOptions>
    {
        public BasicAuthenticationChallengeContext(
            HttpContext context,
            AuthenticationScheme scheme,
            BasicAuthenticationOptions options,
            AuthenticationProperties properties)
            : base(context, scheme, options, properties) { }

        /// <summary>
        /// Any failures encountered during the authentication process.
        /// </summary>
        public Exception AuthenticateFailure { get; set; }

        /// <summary>
        /// If true, will skip any default logic for this challenge.
        /// </summary>
        public bool Handled { get; private set; }

        /// <summary>
        /// Skips any default logic for this challenge.
        /// </summary>
        public void HandleResponse() => Handled = true;
    }
}
