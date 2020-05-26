using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace JwtTokenizer
{
    public class TokenProviderOptionsWithIdentifier : TokenProviderOptions
    {
        /// <summary>
        /// Resolves a user identity given a username and password.
        /// </summary>
        public Func<string, string, Task<ClaimsIdentity>> IdentityResolver { get; set; }
    }
}
