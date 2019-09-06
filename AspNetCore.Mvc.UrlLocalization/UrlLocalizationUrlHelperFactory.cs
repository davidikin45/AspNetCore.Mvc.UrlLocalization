using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace AspNetCore.Mvc.UrlLocalization
{
    public class UrlLocalizationUrlHelperFactory : IUrlHelperFactory
    {
        private readonly IUrlHelperFactory _helperFactory;
        private readonly IUrlLocalizer _urlLocalizer;

        public UrlLocalizationUrlHelperFactory(IUrlHelperFactory helperFactory, IUrlLocalizer urlLocalizer)
        {
            _helperFactory = helperFactory;
            _urlLocalizer = urlLocalizer;
        }

        public IUrlHelper GetUrlHelper(ActionContext context)
        {
            var httpContext = context.HttpContext;

            var urlHelper = _helperFactory.GetUrlHelper(context);
            if (!httpContext.Items.ContainsKey(typeof(UrlLocalizationUrlHelperFactory)))
            {
                urlHelper = new UrlLocalizationUrlHelper(urlHelper, _urlLocalizer);
                httpContext.Items[typeof(IUrlHelper)] = urlHelper;
                httpContext.Items[typeof(UrlLocalizationUrlHelperFactory)] = true;
            }

            return urlHelper;
        }
    }
}
