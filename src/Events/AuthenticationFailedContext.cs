using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace Bazinga.AspNetCore.Authentication.Basic
{
    /// <summary>
    /// Context of a failed basic authentication
    /// </summary>
    public class AuthenticationFailedContext : ResultContext<BasicAuthenticationOptions>
    {
        /// <summary>
        /// The exception thrown while authenticating the request
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// Authentication Failed Context constructor
        /// </summary>
        /// <param name="context">HttpContext of the request containing the Authorize header</param>
        /// <param name="scheme">The auth scheme</param>
        /// <param name="options">Basic auth options</param>
        public AuthenticationFailedContext(
            HttpContext context,
            AuthenticationScheme scheme,
            BasicAuthenticationOptions options)
            : base(context, scheme, options) { }
    }
}
