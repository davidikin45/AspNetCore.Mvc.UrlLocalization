using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Localization.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AspNetCore.Mvc.UrlLocalization
{
    public class RedirectUnsupportedUrlCulturesMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly RedirectUnsupportedUrlCulturesOptions _options;
        static readonly Regex CultureConstraint = new Regex(@"^[a-zA-Z]{2,3}(\-[a-zA-Z]{4})?(\-[a-zA-Z0-9]{2,3})?$", RegexOptions.Compiled);

        public RedirectUnsupportedUrlCulturesMiddleware(
            RequestDelegate next,
            RedirectUnsupportedUrlCulturesOptions redirectUnsupportedCultureOptions)
        {
            _next = next;
            _options = redirectUnsupportedCultureOptions;
        }

        public async Task Invoke(HttpContext context)
        {
            var requestedCulture = GetUrlRequestedCulture(context);

            var cultureFeature = context.Features.Get<IRequestCultureFeature>();

            var actualCulture = cultureFeature?.RequestCulture.Culture.Name;

            // if ((_options.RedirectCulturelessToDefaultCulture && string.IsNullOrEmpty(requestedCulture)) || (_options.RedirectUnspportedCulturesToDefaultCulture && !string.IsNullOrEmpty(requestedCulture) && !string.Equals(requestedCulture, actualCulture, StringComparison.OrdinalIgnoreCase)))
            if ((_options.RedirectUnspportedCulturesToDefaultCulture && !string.IsNullOrEmpty(requestedCulture) && !string.Equals(requestedCulture, actualCulture, StringComparison.OrdinalIgnoreCase)))
            {
                var newCulturedPath = GetNewPath(context, context.Request.Path + context.Request.QueryString.ToString(), requestedCulture, actualCulture);
                context.Response.Redirect(newCulturedPath);
                return;
            }

            await _next.Invoke(context);
        }

        private static string GetUrlRequestedCulture(HttpContext context)
        {
            string requestedCulture = null;

            var requestLocalizationOptions = context.RequestServices.GetService<RequestLocalizationOptions>();
            if(requestLocalizationOptions != null)
            {
                var provider = requestLocalizationOptions.RequestCultureProviders
                  .Select(x => x as RouteDataRequestCultureProvider)
                  .Where(x => x != null)
                  .FirstOrDefault();

                if(provider != null)
                    requestedCulture = context.GetRouteValue(provider.RouteDataStringKey)?.ToString();
            }

            if (requestedCulture == null)
            {
                requestedCulture = GetUrlRequestedCultureFromPath(context.Request.Path);
            }

            return requestedCulture;
        }

        public static string GetUrlRequestedCultureFromPath(string path)
        {
            string requestedCulture = null;

            var pathSegments = path.Split('?')[0].Split('/');
            if (CultureConstraint.IsMatch(pathSegments[1]))
                requestedCulture = pathSegments[1];

            return requestedCulture;
        }

        public static string GetNewPath(HttpContext context, string requestPathAndQueryString, string requestedCulture, string newCulture)
        {
            var routeOptions = context.RequestServices.GetRequiredService<IOptions<RouteOptions>>().Value;
            if (routeOptions.LowercaseUrls)
                newCulture = newCulture.ToLowerInvariant();

            var culturePath = $"/{newCulture}";

            var localizationOptions = context.RequestServices.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value;
            var redirectOptions = context.RequestServices.GetRequiredService<IOptions<RedirectUnsupportedUrlCulturesOptions>>().Value;
            if (!redirectOptions.RedirectCulturelessToDefaultCulture
                && localizationOptions.DefaultRequestCulture != null
                && localizationOptions.DefaultRequestCulture.Culture.Name.Equals(newCulture, StringComparison.InvariantCultureIgnoreCase))
            {
                culturePath = "";
                newCulture = "";
            }
            else
            {
                if (requestPathAndQueryString == "/" || requestPathAndQueryString.StartsWith("/?"))
                    requestPathAndQueryString = requestPathAndQueryString.Substring(1);
            }

            if (string.IsNullOrEmpty(requestedCulture))
            {
                var path = $"{culturePath}{requestPathAndQueryString}";
                return $"{context.Request.PathBase}{path}";
            }
            else
            {
                var path = ReplaceFirstOccurrence(requestPathAndQueryString, requestedCulture, newCulture);
                path = path.Replace($"//", "/");
                return $"{context.Request.PathBase}{path}";
            }
        }

        private static string ReplaceFirstOccurrence(string source, string find, string replace)
        {
            int index = source.IndexOf(find, StringComparison.InvariantCultureIgnoreCase);
            if(index > -1)
            {
                string result = source.Remove(index, find.Length).Insert(index, replace);
                return result;
            }
            else
            {
                return source;
            }
        }
    }

    public class RedirectUnsupportedUrlCulturesOptions
    {
        public bool RedirectCulturelessToDefaultCulture { get; set; } = false;
        public bool RedirectUnspportedCulturesToDefaultCulture { get; set; } = true;
    }
}
