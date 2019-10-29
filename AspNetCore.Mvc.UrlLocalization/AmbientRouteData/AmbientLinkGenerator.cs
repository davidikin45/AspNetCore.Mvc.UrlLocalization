using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AspNetCore.Mvc.UrlLocalization.AmbientRouteData
{
    public class AmbientLinkGenerator : LinkGenerator, IDisposable
    {
        private readonly LinkGenerator _generator;
        private readonly AmbientRouteDataUrlHelperFactoryOptions _options;

        public AmbientLinkGenerator(LinkGenerator generator, IOptions<AmbientRouteDataUrlHelperFactoryOptions> options)
        {
            _generator = generator;
            _options = options.Value;
        }

        public override string GetPathByAddress<TAddress>(HttpContext httpContext, TAddress address, RouteValueDictionary values, RouteValueDictionary ambientValues = null, PathString? pathBase = null, FragmentString fragment = default, LinkOptions options = null)
        {
            var nonRoundTripUsingQueryStringValues = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);

            var routeValues = GetValuesDictionary(values);

            foreach (var routeDataStringKey in _options.AmbientRouteDataKeys)
            {
                if (!routeValues.ContainsKey(routeDataStringKey.RouteDataKey) &&
                  ambientValues.TryGetValue(routeDataStringKey.RouteDataKey, out var value))
                {
                    if (!routeDataStringKey.RoundTripUsingQueryString)
                    {
                        nonRoundTripUsingQueryStringValues.Add(routeDataStringKey.RouteDataKey, value);
                    }

                    routeValues[routeDataStringKey.RouteDataKey] = value;
                }
                else if (!routeValues.ContainsKey(routeDataStringKey.RouteDataKey) && httpContext.Request.Query.TryGetValue(routeDataStringKey.RouteDataKey, out var queryStringValues))
                {
                    if (!routeDataStringKey.RoundTripUsingQueryString)
                    {
                        nonRoundTripUsingQueryStringValues.Add(routeDataStringKey.RouteDataKey, queryStringValues.First());
                    }

                    routeValues[routeDataStringKey.RouteDataKey] = queryStringValues.First();
                }
            }

            var url = _generator.GetPathByAddress<TAddress>(httpContext, address, routeValues, ambientValues, pathBase, fragment, options);

            if (url != null)
            {
                var uri = new Uri(url, UriKind.RelativeOrAbsolute);
                if (!uri.IsAbsoluteUri)
                    uri = new Uri($"http://www.domain.com{url}");

                var queryDictionary = QueryHelpers.ParseQuery(uri.Query);

                if (queryDictionary.Keys.Any(k => nonRoundTripUsingQueryStringValues.ContainsKey(k)))
                {
                    foreach (var key in queryDictionary.Keys.Where(k => nonRoundTripUsingQueryStringValues.ContainsKey(k)))
                    {
                        routeValues.Remove(key);
                    }

                    url = _generator.GetPathByAddress<TAddress>(httpContext, address, routeValues, ambientValues, pathBase, fragment, options);
                }
            }

            return url;
        }

        private RouteValueDictionary GetValuesDictionary(object values)
        {
            if (values is RouteValueDictionary routeValuesDictionary)
            {
                var routeValueDictionary = new RouteValueDictionary();
                foreach (var kvp in routeValuesDictionary)
                {
                    routeValueDictionary.Add(kvp.Key, kvp.Value);
                }

                return routeValueDictionary;
            }

            if (values is IDictionary<string, object> dictionaryValues)
            {

                var routeValueDictionary = new RouteValueDictionary();
                foreach (var kvp in dictionaryValues)
                {
                    routeValueDictionary.Add(kvp.Key, kvp.Value);
                }

                return routeValueDictionary;
            }

            return new RouteValueDictionary(values);
        }

        public override string GetPathByAddress<TAddress>(TAddress address, RouteValueDictionary values, PathString pathBase = default, FragmentString fragment = default, LinkOptions options = null)
        {
            return _generator.GetPathByAddress<TAddress>(address, values, pathBase, fragment, options);
        }

        public override string GetUriByAddress<TAddress>(HttpContext httpContext, TAddress address, RouteValueDictionary values, RouteValueDictionary ambientValues = null, string scheme = null, HostString? host = null, PathString? pathBase = null, FragmentString fragment = default, LinkOptions options = null)
        {
            return _generator.GetUriByAddress<TAddress>(httpContext, address, values, ambientValues, scheme, host, pathBase, fragment, options);
        }

        public override string GetUriByAddress<TAddress>(TAddress address, RouteValueDictionary values, string scheme, HostString host, PathString pathBase = default, FragmentString fragment = default, LinkOptions options = null)
        {
            return _generator.GetUriByAddress<TAddress>(address, values, scheme, host, pathBase, fragment, options);
        }
        public void Dispose()
        {
            if (_generator is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}
