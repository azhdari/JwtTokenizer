using JwtTokenizer;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class JwtTokenizerServiceCollectionExtensions
    {
        public static IServiceCollection AddJwtTokenizer(this IServiceCollection services, Action<TokenProviderOptions> setupAction)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (setupAction != null)
            {
                services.Configure(setupAction);
            }

            services.AddScoped<JwtService>();
            return services;
        }
    }
}
