using AspNetCore.Mvc.UrlLocalization;
using Microsoft.AspNetCore.Mvc;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class UrlLocalizationMvcOptionsExtensions
    {

        public static MvcOptions AddOptionalCultureAttributeRouteConvention(this MvcOptions options, string cultureRouteDataStringKey = "culture", string cultureConstraintKey = "cultureCheck")
        {
            options.Conventions.Insert(0, new UrlLocalizationConvention(true, cultureRouteDataStringKey, cultureConstraintKey));

            return options;
        }

        public static MvcOptions AddCultureAttributeRouteConvention(this MvcOptions options, string cultureRouteDataStringKey = "culture", string cultureConstraintKey = "cultureCheck")
        {
            options.Conventions.Insert(0, new UrlLocalizationConvention(false, cultureRouteDataStringKey, cultureConstraintKey));

            return options;
        }
    }
}
