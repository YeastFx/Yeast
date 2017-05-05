using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace Yeast.Security.Jwt
{
    public class JwtIssuer
    {
        private readonly JwtIssuerOptions _options;

        public JwtIssuer(IOptions<JwtIssuerOptions> options) {
            _options = options.Value;
        }

        public string GetJwt(ClaimsIdentity identity, string jwtIdentifier = null, TimeSpan? tokenLifetime = null)
        {
            if(identity == null)
            {
                throw new ArgumentNullException(nameof(identity));
            }

            var jwtClaims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, identity.Name, ClaimValueTypes.String),
                new Claim(JwtRegisteredClaimNames.Jti, jwtIdentifier ?? Guid.NewGuid().ToString(), ClaimValueTypes.String),
                new Claim(JwtRegisteredClaimNames.Iat, EpochTime.GetIntDate(DateTime.Now).ToString(),ClaimValueTypes.Integer64)
            };

            DateTime? expirationDate = null;
            if (tokenLifetime.HasValue)
            {
                expirationDate = DateTime.Now.Add(tokenLifetime.Value);
            }
            else if (_options.DefaultTokenLifetime.HasValue)
            {
                expirationDate = DateTime.Now.Add(_options.DefaultTokenLifetime.Value);
            }

            // Creates the JWT security token and encode it.
            var jwt = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: jwtClaims.Concat(identity.Claims),
                notBefore: DateTime.Now,
                expires: expirationDate,
                signingCredentials: _options.SigningCredentials);

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
    }
}
