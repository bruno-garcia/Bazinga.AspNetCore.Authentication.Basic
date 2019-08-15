using Microsoft.AspNetCore.Authentication;

namespace Bazinga.AspNetCore.Authentication.Basic
{
    /// <summary>
    /// Options class provides information needed to control Basic Authentication handler behavior
    /// </summary>
    public class BasicAuthenticationOptions : AuthenticationSchemeOptions
    {
        /// <summary>
        /// Gets or sets the Realm
        /// </summary>
        /// <seealso href="https://tools.ietf.org/html/rfc2617#section-1.2"/>
        /// <value>
        /// The Realm 
        /// </value>
        public string? Realm { get; set; }

        /// <summary>
        /// Gets the challenge to put in the "WWW-Authenticate" header.
        /// </summary>
        public string Challenge => $"{BasicAuthenticationDefaults.AuthenticationScheme} realm=\"{Realm}\"";

        /// <summary>
        /// The object provided by the application to process events raised by the basic authentication handler.
        /// </summary>
        public new BasicAuthenticationEvents Events
        {
            get => (BasicAuthenticationEvents)base.Events;
            set => base.Events = value;
        }
    }
}