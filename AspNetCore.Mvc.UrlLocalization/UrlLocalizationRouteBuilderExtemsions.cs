using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace AspNetCore.Mvc.UrlLocalization
{
    public static class UrlLocalizationRouteBuilderExtemsions
    {
        //404 cultureless. Used with AddCultureRouteConvention.
        public static IRouteBuilder RedirectCulturelessToDefaultCulture(this IRouteBuilder routes, string cultureRouteDataStringKey = "culture", string cultureConstraintKey = "cultureCheck")
        {
            //any route 1.has a culture; and 2.does not match the previous global route url will return a 404.
            routes.MapGet("{" + cultureRouteDataStringKey + (!string.IsNullOrEmpty(cultureConstraintKey) ? $":{cultureConstraintKey}" : "") + "}/{**path}", context =>
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                return Task.CompletedTask;
            });

            //redirect culture-less routes
            routes.MapGet("{**path}", (RequestDelegate)(ctx =>
            {
                var cultureFeature = ctx.Features.Get<IRequestCultureFeature>();   //Available after app.UseRequestLocalization(); Could also use CultureInfo.CurrentCulture or CultureInfo.CurrentUICulture.
                var actualCulture = cultureFeature?.RequestCulture.Culture.Name;
                var actualCultureLanguage = cultureFeature?.RequestCulture.Culture.TwoLetterISOLanguageName;

                var routeOptions = ctx.RequestServices.GetRequiredService<IOptions<RouteOptions>>().Value;
                if (routeOptions.LowercaseUrls)
                    actualCulture = actualCulture.ToLowerInvariant();

                var path = (ctx.GetRouteValue("path") ?? string.Empty).ToString();
                path = (!string.IsNullOrEmpty(path) ? "/" : string.Empty) + path;

                var culturedPath = $"{ctx.Request.PathBase}/{actualCulture}{path}{ctx.Request.QueryString.ToString()}";

                ctx.Response.Redirect(culturedPath);
                return Task.CompletedTask;
            }));

            return routes;
        }

#if NETCOREAPP3_0
        public static IEndpointRouteBuilder RedirectCulturelessToDefaultCulture(this IEndpointRouteBuilder routes, string cultureRouteDataStringKey = "culture", string cultureConstraintKey = "cultureCheck")
        {
            //any route 1.has a culture; and 2.does not match the previous global route url will return a 404.
            routes.MapGet("{" + cultureRouteDataStringKey + (!string.IsNullOrEmpty(cultureConstraintKey) ? $":{cultureConstraintKey}" : "") + "}/{**path}", context =>
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                return Task.CompletedTask;
            });

            //redirect culture-less routes
            routes.MapGet("{**path}", (RequestDelegate)(ctx =>
            {

                var cultureFeature = ctx.Features.Get<IRequestCultureFeature>();   //Available after app.UseRequestLocalization(); Could also use CultureInfo.CurrentCulture or CultureInfo.CurrentUICulture.

                var actualCulture = cultureFeature?.RequestCulture.Culture.Name;
                var actualCultureLanguage = cultureFeature?.RequestCulture.Culture.TwoLetterISOLanguageName;
        
                var routeOptions = ctx.RequestServices.GetRequiredService<IOptions<RouteOptions>>().Value;
                if (routeOptions.LowercaseUrls)
                    actualCulture = actualCulture.ToLowerInvariant();

                var path = (ctx.GetRouteValue("path") ?? string.Empty).ToString();
                path = (!string.IsNullOrEmpty(path) ? "/" : string.Empty) + path;

                var culturedPath = $"{ctx.Request.PathBase}/{actualCulture}{path}{ctx.Request.QueryString.ToString()}";
                ctx.Response.Redirect(culturedPath);
                return Task.CompletedTask;
            }));

            return routes;
        }
#endif
    }
}
