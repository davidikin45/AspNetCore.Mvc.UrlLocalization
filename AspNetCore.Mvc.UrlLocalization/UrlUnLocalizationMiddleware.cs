﻿using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace AspNetCore.Mvc.UrlLocalization
{
    public class UrlUnlocalizationMiddleware
    {
        private readonly IUrlLocalizer _urlLocalizer;
        private readonly RequestDelegate _next;
        private readonly UrlUnlocalizationOptions _options;

        public UrlUnlocalizationMiddleware(RequestDelegate next, IUrlLocalizer urlLocalizer, UrlUnlocalizationOptions options)
        {
            _next = next;
            _urlLocalizer = urlLocalizer;
            _options = options;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            //var cultureFeature = context.Features.Get<IRequestCultureFeature>();   //Available after app.UseRequestLocalization(); Could also use CultureInfo.CurrentCulture or CultureInfo.CurrentUICulture.
            //var actualCultureLanguage = cultureFeature?.RequestCulture.UICulture.TwoLetterISOLanguageName;

            var originalPath = context.Request.Path;

            if (!context.Items.ContainsKey("UrlUnlocalized"))
            {
                context.Items.Add("UrlUnlocalized", true);

                if(_options.NonLocalizedUrlHandling == NonLocalizedUrlHandling.Redirect || _options.NonLocalizedUrlHandling == NonLocalizedUrlHandling.Status404NotFound)
                {
                    var localizedPath = _urlLocalizer.LocalizeRequestPath(context.Request.Path.Value);
                    if (!localizedPath.Equals(context.Request.Path.Value, StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (_options.NonLocalizedUrlHandling == NonLocalizedUrlHandling.Status404NotFound)
                        {
                            context.Response.StatusCode = StatusCodes.Status404NotFound;
                            return;
                        }
                        else if (_options.NonLocalizedUrlHandling == NonLocalizedUrlHandling.Redirect)
                        {
                            var uri = new Uri($"{context.Request.Scheme}://{context.Request.Host}{context.Request.PathBase}{localizedPath}{context.Request.QueryString.ToString()}");
                            context.Response.Redirect(uri.AbsoluteUri);
                            return;
                        }
                    }
                }

                var unlocalizedPath = _urlLocalizer.UnlocalizeRequestPath(context.Request.Path.Value);
                if (!unlocalizedPath.Equals(context.Request.Path.Value, StringComparison.InvariantCultureIgnoreCase))
                {
                    context.Request.Path = unlocalizedPath;

                }
            }

            await _next(context);

            //replace the original url after the remaining middleware has finished processing
            context.Request.Path = originalPath;
        }
    }

    public class UrlUnlocalizationOptions
    {
        public NonLocalizedUrlHandling NonLocalizedUrlHandling { get; set; } = NonLocalizedUrlHandling.Redirect;

    }

    public enum NonLocalizedUrlHandling
    {
        Redirect,
        Status404NotFound,
        ContinueProcessing
    }
}
