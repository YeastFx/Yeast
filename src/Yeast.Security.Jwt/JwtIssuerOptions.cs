using Microsoft.IdentityModel.Tokens;
using System;

namespace Yeast.Security.Jwt
{
    public class JwtIssuerOptions
    {
        /// <summary>
        /// Issuer identifier
        /// </summary>
        /// <remarks>https://tools.ietf.org/html/rfc7519#section-4.1.1</remarks>
        public string Issuer { get; set; }

        /// <summary>
        /// Audience identifier
        /// </summary>
        /// <remarks>https://tools.ietf.org/html/rfc7519#section-4.1.3</remarks>
        public string Audience { get; set; }

        /// <summary>
        /// Credentials used to sign tokens
        /// </summary>
        public SigningCredentials SigningCredentials { get; set; }

        /// <summary>
        /// The period of time the token remains valid after being issued.
        /// </summary>
        public TimeSpan? DefaultTokenLifetime { get; set; }
    }
}
