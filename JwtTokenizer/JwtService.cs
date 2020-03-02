using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace JwtTokenizer
{
    public class JwtService
    {
        private readonly TokenProviderOptions _options;
        private readonly ILogger _logger;

        public JwtService(
            IOptions<TokenProviderOptions> options,
            ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<JwtService>();

            _options = options.Value;
            JwtHelper.ThrowIfInvalidOptions(_options);
        }

        public async Task<TokenResultDto> Prepare(ClaimsIdentity identity, string username)
        {
            try
            {
                var now = DateTime.UtcNow;

                /* Specifically add the jti (nonce), iat (issued timestamp), and sub (subject/user) claims.
                 * You can add other claims here, if you want: */

                var claims = identity.Claims?.ToList() ?? new List<Claim>();
                claims.AddRange(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, username),
                    new Claim(JwtRegisteredClaimNames.Jti, await _options.NonceGenerator()),
                    new Claim(JwtRegisteredClaimNames.Iat, JwtHelper.ToUnixEpochDate(now).ToString(), ClaimValueTypes.Integer64),
                });

                // Create the JWT and write it to a string
                var jwt = new JwtSecurityToken(
                    issuer: _options.Issuer,
                    audience: _options.Audience,
                    claims: claims.ToArray(),
                    notBefore: now,
                    expires: now.Add(_options.Expiration),
                    signingCredentials: _options.SigningCredentials);
                var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

                var response = new TokenResultDto
                {
                    Token = encodedJwt,
                    ExpiresIn = (int)_options.Expiration.TotalSeconds,
                };

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unable to create JWT token: {ex.Message}");
                throw;
            }
        }
    }
}
