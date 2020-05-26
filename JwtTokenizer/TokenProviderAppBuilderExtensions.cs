using JwtTokenizer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// Adds a token generation endpoint to an application pipeline.
    /// </summary>
    public static class TokenProviderAppBuilderExtensions
    {
        /// <summary>
        /// Adds the <see cref="TokenProviderMiddleware"/> middleware to the specified <see cref="IApplicationBuilder"/>, which enables token generation capabilities.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/> to add the middleware to.</param>
        /// <param name="configure"></param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IApplicationBuilder UseJwtTokenizer(this IApplicationBuilder app, Func<Func<string, string, Task<ClaimsIdentity>>> configure)
        {
            using (IServiceScope scope = app.ApplicationServices.CreateScope())
            {
                IServiceProvider serviceProvider = scope.ServiceProvider;
                Func<string, string, Task<ClaimsIdentity>> func = configure();
                TokenProviderOptions tokenProviderOptions = serviceProvider.GetService<IOptions<TokenProviderOptions>>().Value;

                TokenProviderOptionsWithIdentifier optionsWithIdentifier = new TokenProviderOptionsWithIdentifier
                {
                    Audience = tokenProviderOptions.Audience,
                    Expiration = tokenProviderOptions.Expiration,
                    Issuer = tokenProviderOptions.Issuer,
                    NonceGenerator = tokenProviderOptions.NonceGenerator,
                    Path = tokenProviderOptions.Path,
                    SigningCredentials = tokenProviderOptions.SigningCredentials,
                    IdentityResolver = func
                };

                TokenProviderOptionsWithIdentifier options = optionsWithIdentifier;

                return app.UseMiddleware<TokenProviderMiddleware>(Options.Create(options));
            }
        }
    }
}
