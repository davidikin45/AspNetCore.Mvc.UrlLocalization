using AspNetCore.Mvc.UrlLocalization;
using AspNetCore.Mvc.UrlLocalization.AmbientRouteData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class UrlLocalizationMvcBuilderExtensions
    {
        #region Url Localization
        /// <summary>
        /// Adds the action link localization to the application.
        /// </summary>
        public static IMvcBuilder AddActionLinkLocalization(this IMvcBuilder builder)
        {
            var services = builder.Services;

            services.AddUrlLocalization();
            services.Decorate<IUrlHelperFactory, UrlLocalizationUrlHelperFactory>();

            return builder;
        }

        public static IMvcBuilder AddOptionalCultureRouteConvention(this IMvcBuilder builder, string cultureRouteDataStringKey = "culture", string cultureConstraintKey = "cultureCheck")
        {
            var services = builder.Services;

            services.AddCultureRouteConstraint(cultureConstraintKey);
            services.Configure<MvcOptions>(options =>
            {
                options.AddOptionalCultureAttributeRouteConvention(cultureRouteDataStringKey, cultureConstraintKey);
            });

            return builder;
        }


        public static IMvcBuilder AddCultureRouteConvention(this IMvcBuilder builder, string cultureRouteDataStringKey = "culture", string cultureConstraintKey = "cultureCheck")
        {
            var services = builder.Services;

            services.AddCultureRouteConstraint(cultureConstraintKey);
            services.Configure<MvcOptions>(options =>
            {
                options.AddCultureAttributeRouteConvention(cultureRouteDataStringKey, cultureConstraintKey);
            });

            return builder;
        }
        #endregion

        #region Ambient Route Data
        /// <summary>
        /// Adds ambient route data URL helper factory service to the application.
        /// </summary>
        public static IMvcBuilder AddAmbientRouteDataUrlHelperFactory(this IMvcBuilder builder, Action<AmbientRouteDataUrlHelperFactoryOptions> setupAction = null)
        {
            var services = builder.Services;

            if (setupAction != null)
                services.Configure(setupAction);

            services.Decorate<IUrlHelperFactory, AmbientRouteDataUrlHelperFactory>();
            services.Decorate<LinkGenerator, AmbientLinkGenerator>();

            return builder;
        }
        #endregion
    }
}
