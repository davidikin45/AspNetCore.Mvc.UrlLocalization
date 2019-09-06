using AspNetCore.Mvc.UrlLocalization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class UrlLocalizationServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the URL localizer to the application.
        /// </summary>
        public static IServiceCollection AddUrlLocalization(this IServiceCollection services)
        {
            services.TryAddSingleton<IUrlLocalizer, UrlLocalizer>();
            return services;
        }

        public static IServiceCollection ConfigureRedirectUnsupportedUrlCulturesOptions(this IServiceCollection services, Action<RedirectUnsupportedUrlCulturesOptions> configureOptions)
        {
            services.Configure(configureOptions);

            services.AddSingleton(sp => sp.GetService<IOptions<RedirectUnsupportedUrlCulturesOptions>>().Value);

            return services;
        }

        public static IServiceCollection AddCultureRouteConstraint(this IServiceCollection services, string cultureConstraintKey = "cultureCheck")
        {
            return services.Configure<RouteOptions>(options =>
            {
                if (!options.ConstraintMap.ContainsKey(cultureConstraintKey))
                {
                    options.ConstraintMap.Add(cultureConstraintKey, typeof(CultureConstraint));
                }
            });
        }
    }
}
