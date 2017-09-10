using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace Bazinga.AspNetCore.Authentication.Basic
{
    /// <summary>
    /// Context of a successful basic authentication
    /// </summary>
    public class AuthenticationSucceededContext : ResultContext<BasicAuthenticationOptions>
    {
        /// <summary>
        /// The user id which successfully authenticated
        /// </summary>
        public string UserId { get; }

        /// <summary>
        /// Authentication Succeeded Context constructor
        /// </summary>
        /// <param name="userId">The user id which successfully authenticated</param>
        /// <param name="context">HttpContext of the request containing the Authorize header</param>
        /// <param name="scheme">The auth scheme</param>
        /// <param name="options">Basic auth options</param>
        public AuthenticationSucceededContext(
            string userId,
            HttpContext context,
            AuthenticationScheme scheme,
            BasicAuthenticationOptions options)
            : base(context, scheme, options)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
        }
    }
}
