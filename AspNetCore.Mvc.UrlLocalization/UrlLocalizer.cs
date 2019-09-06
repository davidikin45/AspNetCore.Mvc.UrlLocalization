using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AspNetCore.Mvc.UrlLocalization
{

    //https://github.com/aspnet/Localization/blob/43b974482c7b703c92085c6f68b3b23d8fe32720/src/Microsoft.Extensions.Localization/ResourceManagerStringLocalizerFactory.cs
    //https://github.com/aspnet/Localization/blob/master/src/Microsoft.Extensions.Localization/ResourceManagerStringLocalizer.cs
    public class UrlLocalizer : IUrlLocalizer
    {
        private readonly IStringLocalizer _localizer;
        private readonly UrlLocalizerOptions _urlLocalizerOptions;
        private readonly RouteOptions _routeOptions;
        public UrlLocalizer(IStringLocalizerFactory stringLocalizerFactory, IOptions<UrlLocalizerOptions> urlLocalizerOptions, IOptions<RouteOptions> routeOptions)
        {
            _localizer = stringLocalizerFactory.Create(urlLocalizerOptions.Value.ResourceName, Assembly.GetEntryAssembly().GetName().Name);
            _urlLocalizerOptions = urlLocalizerOptions.Value;
            _routeOptions = routeOptions.Value;
        }

        public UrlLocalizer(IStringLocalizer stringLocalizer, UrlLocalizerOptions urlLocalizerOptions, RouteOptions routeOptions)
        {
            _localizer = stringLocalizer;
            _urlLocalizerOptions = urlLocalizerOptions;
            _routeOptions = routeOptions;
        }

        //Can Make MultiLanguageString String similar to LocalizedString
        public LocalizedString this[string name] => _localizer[name];

        public LocalizedString this[string name, params object[] arguments] => _localizer[name, arguments];

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            return _localizer.GetAllStrings(includeParentCultures);
        }
        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            return new UrlLocalizer(_localizer.WithCulture(culture), _urlLocalizerOptions,  _routeOptions);
        }

        public string LocalizeLink(string path)
        {
            if (path == null)
                return path;

            var builder = new StringBuilder();

            var pathSegments = path.Split('/');

            TextInfo ti = CultureInfo.CurrentCulture.TextInfo;
            for (int i = 0; i < pathSegments.Length; i++)
            {
                var pathSegment = pathSegments[i];

                var translatedPathSegment = this[pathSegment].ToString();
                if (translatedPathSegment == pathSegment)
                {
                    var titlecase = this[ti.ToTitleCase(pathSegment)];
                    if (titlecase != ti.ToTitleCase(pathSegment))
                        translatedPathSegment = ti.ToLower(titlecase);
                }

                builder.Append(translatedPathSegment);

                if (i != pathSegments.Length - 1)
                    builder.Append("/");
            }

            return builder.ToString();
        }

        static ConcurrentDictionary<string, Dictionary<string, string>> ReverseLookups = new ConcurrentDictionary<string, Dictionary<string, string>>();

        public string UnlocalizeRequestPath(string path)
        {
            if (path == null)
                return path;

            var pathSegments = path.Split('/');

            TextInfo ti = CultureInfo.CurrentCulture.TextInfo;

            Dictionary<string, string> reverseLookup = ReverseLookups.GetOrAdd(CultureInfo.CurrentUICulture.Name, culture =>
            {
                try
                {
                    //Spanish > English
                    return this.GetAllStrings().ToDictionary(ls => ls.Value, ls => ls.Name);
                }
                catch
                {
                    return new Dictionary<string, string>();
                }
            });

            var builder = new StringBuilder();


            for (int i = 0; i < pathSegments.Length; i++)
            {
                var pathSegment = pathSegments[i];

                var translatedPathSegment = reverseLookup.ContainsKey(pathSegment) ? reverseLookup[pathSegment] : reverseLookup.ContainsKey(ti.ToTitleCase(pathSegment)) ? reverseLookup[ti.ToTitleCase(pathSegment)] : pathSegment;

                if (_routeOptions.LowercaseUrls)
                    translatedPathSegment = translatedPathSegment.ToLowerInvariant();

                builder.Append(translatedPathSegment);

                if (i != pathSegments.Length - 1)
                    builder.Append("/");
            }

            return builder.ToString();
        }

        public string LocalizeRequestPath(string path)
        {
            if (path == null)
                return path;

            var pathSegments = path.Split('/');

            //English > Spanish
            TextInfo ti = CultureInfo.CurrentCulture.TextInfo;
            for (int i = 0; i < pathSegments.Length; i++)
            {
                var pathSegment = pathSegments[i];

                var translatedPathSegment = this[pathSegment];
                if (translatedPathSegment == pathSegment)
                {
                    var titlecase = this[ti.ToTitleCase(pathSegment)];
                    if (titlecase != ti.ToTitleCase(pathSegment))
                    {
                        pathSegments[i] = ti.ToLower(titlecase);
                    }
                    else
                    {

                    }
                }
                else
                {
                    pathSegments[i] = translatedPathSegment;
                }
            }

            var builder = new StringBuilder();

            for (int i = 0; i < pathSegments.Length; i++)
            {
                var pathSegment = pathSegments[i];

                builder.Append(pathSegment);

                if (i != pathSegments.Length - 1)
                    builder.Append("/");
            }

            return builder.ToString();
        }
    }

    public class UrlLocalizerOptions
    {
        public string ResourceName { get; set; } = "Url";
    }
}
