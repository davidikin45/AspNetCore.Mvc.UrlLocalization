using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace AspNetCore.Mvc.UrlLocalization.AmbientRouteData
{
    //https://stackoverflow.com/questions/52536969/asp-net-core-urlhelper-and-how-it-works
    public class AmbientRouteDataUrlHelperFactory : IUrlHelperFactory
    {
        private readonly IUrlHelperFactory _helperFactory;
        private readonly AmbientRouteDataUrlHelperFactoryOptions _options;

        public AmbientRouteDataUrlHelperFactory(IUrlHelperFactory helperFactory, IOptions<AmbientRouteDataUrlHelperFactoryOptions> options)
        {
            _helperFactory = helperFactory;
            _options = options.Value;
        }

        public IUrlHelper GetUrlHelper(ActionContext context)
        {
            var httpContext = context.HttpContext;

            var urlHelper = _helperFactory.GetUrlHelper(context);
            if (!httpContext.Items.ContainsKey(typeof(AmbientRouteDataUrlHelperFactory)))
            {
                urlHelper = new AmbientRouteDataUrlHelper(urlHelper, _options);
                httpContext.Items[typeof(IUrlHelper)] = urlHelper;
                httpContext.Items[typeof(AmbientRouteDataUrlHelperFactory)] = true;
            }

            return urlHelper;
        }
    }

    public class AmbientRouteDataUrlHelperFactoryOptions
    {
        public List<AmbientRouteDataKey> AmbientRouteDataKeys = new List<AmbientRouteDataKey>() { };
    }

    public class AmbientRouteDataKey
    {
        public AmbientRouteDataKey(string routeDataKey, bool roundTripUsingQueryString)
        {
            RouteDataKey = routeDataKey;
            RoundTripUsingQueryString = roundTripUsingQueryString;
        }

        public string RouteDataKey { get; set; }
        public bool RoundTripUsingQueryString { get; set; } = false;
    }
}
