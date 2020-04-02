using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Routing;
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

                var culturedPath = UrlLocalizationHelper.GetCulturedRedirectUrl(ctx, $"{ctx.Request.Path.ToString()}{ctx.Request.QueryString.ToString()}", actualCulture);

                ctx.Response.Redirect(culturedPath);
                return Task.CompletedTask;
            }));

            return routes;
        }

#if NETCOREAPP3_0
        public static IEndpointRouteBuilder RedirectCulturelessToDefaultCulture(this IEndpointRouteBuilder routes, string cultureRouteDataStringKey = "culture", string cultureConstraintKey = "cultureCheck")
        {
            //any route 1.has a culture; and 2.does not match the previous global route url will return a 404.
            var conventionBuilder = routes.MapGet("{" + cultureRouteDataStringKey + (!string.IsNullOrEmpty(cultureConstraintKey) ? $":{cultureConstraintKey}" : "") + "}/{**path}", context =>
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                return Task.CompletedTask;
            });            
            conventionBuilder.Add(b => ((RouteEndpointBuilder)b).Order = int.MaxValue);

            //redirect culture-less routes
            conventionBuilder = routes.MapGet("{**path}", (RequestDelegate)(ctx =>
            {
                var cultureFeature = ctx.Features.Get<IRequestCultureFeature>();   //Available after app.UseRequestLocalization(); Could also use CultureInfo.CurrentCulture or CultureInfo.CurrentUICulture.
                var actualCulture = cultureFeature?.RequestCulture.Culture.Name;

                var culturedPath = UrlLocalizationHelper.GetCulturedRedirectUrl(ctx, $"{ctx.Request.Path.ToString()}{ctx.Request.QueryString.ToString()}", actualCulture);

                ctx.Response.Redirect(culturedPath);
                return Task.CompletedTask;
            }));
            conventionBuilder.Add(b => ((RouteEndpointBuilder)b).Order = int.MaxValue);

            return routes;
        }
#endif
    }
}
