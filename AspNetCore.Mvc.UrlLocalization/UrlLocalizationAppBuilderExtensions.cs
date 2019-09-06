using AspNetCore.Mvc.UrlLocalization;
using System;

namespace Microsoft.AspNetCore.Builder
{
    public static class UrlLocalizationAppBuilderExtensions
    {
        /// <summary>
        /// Uses the URL localization.
        /// </summary>
        public static IApplicationBuilder UseUrlUnlocalization(this IApplicationBuilder app, Action<UrlUnlocalizationOptions> configureOptions = null)
        {
            var options = new UrlUnlocalizationOptions();
            if (configureOptions != null)
            {
                configureOptions(options);
            }

            return app.UseUrlUnlocalization(options);
        }

        /// <summary>
        /// Uses the URL localization.
        /// </summary>
        public static IApplicationBuilder UseUrlUnlocalization(this IApplicationBuilder app, UrlUnlocalizationOptions options)
        {
            return app.UseMiddleware<UrlUnlocalizationMiddleware>(options);
        }

        /// <summary>
        /// Redirects Unsupported Cultures.
        /// </summary>
        public static IApplicationBuilder UseRedirectUnsupportedUrlCultures(this IApplicationBuilder app, Action<RedirectUnsupportedUrlCulturesOptions> configureOptions = null)
        {
            var options = new RedirectUnsupportedUrlCulturesOptions();
            if (configureOptions != null)
            {
                configureOptions(options);
            }

            return app.UseRedirectUnsupportedUrlCultures(options);
        }

        /// <summary>
        /// Redirects Unsupported Cultures.
        /// </summary>
        public static IApplicationBuilder UseRedirectUnsupportedUrlCultures(this IApplicationBuilder app, RedirectUnsupportedUrlCulturesOptions options)
        {
            return app.UseMiddleware<RedirectUnsupportedUrlCulturesMiddleware>(options);
        }
    }
}
