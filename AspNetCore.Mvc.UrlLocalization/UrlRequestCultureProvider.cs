using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AspNetCore.Mvc.UrlLocalization
{
    public class UrlRequestCultureProvider : RequestCultureProvider
    {
        static readonly Regex CultureConstraint = new Regex(@"^[a-zA-Z]{2,3}(\-[a-zA-Z]{4})?(\-[a-zA-Z0-9]{2,3})?$", RegexOptions.Compiled);

        public UrlRequestCultureProvider()
        {

        }

        public override Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            string url = httpContext.Request.Path.Value;

            // Example: /fi
            if (url.Length >= 3)
            {
                // Get the /value/value2 value
                string cultureRoute = url.Split('/')[1];
                if (CultureConstraint.IsMatch(cultureRoute))
                {
                    return Task.FromResult(new ProviderCultureResult(cultureRoute));
                }
            }

            return NullProviderCultureResult;
        }
    }
}
