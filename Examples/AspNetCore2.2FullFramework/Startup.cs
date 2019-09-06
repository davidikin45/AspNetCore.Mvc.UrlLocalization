using AspNetCore.Mvc.UrlLocalization;
using AspNetCore.Mvc.UrlLocalization.AmbientRouteData;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Localization.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Globalization;

namespace AspNetCore22FullFramework
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public bool RedirectCulturelessToDefaultCulture = false;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.AddUrlLocalization();
            services.AddCultureRouteConstraint("cultureCheck");

            services.AddMvc(options => {
                //Adds {culture:cultureCheck} to ALL routes
                if (RedirectCulturelessToDefaultCulture)
                    options.AddCultureAttributeRouteConvention("culture", "cultureCheck");
                else
                    options.AddOptionalCultureAttributeRouteConvention("culture", "cultureCheck");

                //options.Filters.Add(new MvcUrlLocalizationFilterAttribute());
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
            .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
            .AddDataAnnotationsLocalization()
            .AddActionLinkLocalization()
            .AddAmbientRouteDataUrlHelperFactory(options =>
            {
                options.AmbientRouteDataKeys.Add(new AmbientRouteDataKey("area", false));
                options.AmbientRouteDataKeys.Add(new AmbientRouteDataKey("culture", true));
                options.AmbientRouteDataKeys.Add(new AmbientRouteDataKey("ui-culture", true));
            });

            var supportedCultures = new[]
           {
                new CultureInfo("en-US"),
                new CultureInfo("es"),
                new CultureInfo("fr"),
                new CultureInfo("de")
            };

            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new RequestCulture("en-US");
                // Formatting numbers, dates, etc.
                options.SupportedCultures = supportedCultures;
                // UI strings that we have localized.
                options.SupportedUICultures = supportedCultures;
                options.RequestCultureProviders = new List<IRequestCultureProvider>()
                {
                     new RouteDataRequestCultureProvider() { Options = options, RouteDataStringKey = "culture", UIRouteDataStringKey = "ui-culture" },
                     new UrlRequestCultureProvider(),
                     new QueryStringRequestCultureProvider() { QueryStringKey = "culture", UIQueryStringKey = "ui-culture" },
                     new CookieRequestCultureProvider(),
                     new AcceptLanguageHeaderRequestCultureProvider(),
                };
            });

            services.AddSingleton(sp => sp.GetService<IOptions<RequestLocalizationOptions>>().Value);

            services.Configure<RedirectUnsupportedUrlCulturesOptions>(options =>
            {
                options.RedirectUnspportedCulturesToDefaultCulture = true;
                options.RedirectCulturelessToDefaultCulture = RedirectCulturelessToDefaultCulture;
            });

            services.AddSingleton(sp => sp.GetService<IOptions<RedirectUnsupportedUrlCulturesOptions>>().Value);

            services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, RequestLocalizationOptions localizationOptions, RedirectUnsupportedUrlCulturesOptions redirectUnsupportedUrlCulturesOptions)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRequestLocalization(localizationOptions);
            app.UseRedirectUnsupportedUrlCultures(redirectUnsupportedUrlCulturesOptions);
            app.UseUrlUnlocalization();

            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                if(redirectUnsupportedUrlCulturesOptions.RedirectCulturelessToDefaultCulture)
                {
                    routes.MapRoute(
                     name: "defaultWithCulture",
                     template: "{culture:cultureCheck}/{controller=Home}/{action=Index}/{id?}");

                    //Other Routes

                    routes.RedirectCulturelessToDefaultCulture();
                }
                else
                {
                    routes.MapRoute(
                     name: "default",
                     template: "{controller=Home}/{action=Index}/{id?}");

                    routes.MapRoute(
                    name: "defaultWithCulture",
                    template: "{culture:cultureCheck}/{controller=Home}/{action=Index}/{id?}");

                    //Other Routes
                }
            });
        }
    }
}
