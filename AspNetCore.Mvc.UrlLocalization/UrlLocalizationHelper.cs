using Microsoft.AspNetCore.Http;

namespace AspNetCore.Mvc.UrlLocalization
{
    public static class UrlLocalizationHelper
    {
        public static string GetCulturedRedirectUrl(HttpContext context, string path, string cultureName)
        {
            var requestedCulture = RedirectUnsupportedUrlCulturesMiddleware.GetUrlRequestedCultureFromPath(path);
            var culturedPath = RedirectUnsupportedUrlCulturesMiddleware.GetNewPath(context, path, requestedCulture, cultureName);
            return culturedPath;
        }
    }
}
