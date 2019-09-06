using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AspNetCore.Mvc.UrlLocalization
{

    //When Endpoiint Routing is used. app.UseEndPointRouting(2.2) or app.UseRouting(3.0) filter only gets applied if valid action is matched.
    public class MvcUrlLocalizationFilterAttribute : MiddlewareFilterAttribute
    {
        public MvcUrlLocalizationFilterAttribute() : base(typeof(MvcUrlLocalizationPipeline))
        {

        }
    }

    public class MvcUrlLocalizationPipeline
    {
        public void Configure(IApplicationBuilder app, IOptions<RequestLocalizationOptions> options, IOptions<RedirectUnsupportedUrlCulturesOptions> redirectUnsupportedUrlCulturesOptions)
        {
            app.UseRequestLocalization(options.Value); // Sets CultureInfo.CurrentCulture and CultureInfo.CurrentUICulture
            app.UseRedirectUnsupportedUrlCultures(redirectUnsupportedUrlCulturesOptions.Value);
        }
    }
}
