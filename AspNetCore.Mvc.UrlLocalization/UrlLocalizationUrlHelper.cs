using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace AspNetCore.Mvc.UrlLocalization
{
    internal class UrlLocalizationUrlHelper : UrlHelperBase
    {
        private readonly IUrlHelper _urlHelper;
        private readonly IUrlLocalizer _urlLocalizer;

        public UrlLocalizationUrlHelper(IUrlHelper urlHelper, IUrlLocalizer urlLocalizer)
            : base(urlHelper.ActionContext)
        {
            _urlHelper = urlHelper;
            _urlLocalizer = urlLocalizer;
        }

        public new ActionContext ActionContext => _urlHelper.ActionContext;

        public override string Action(UrlActionContext actionContext)
        {
            var url = _urlHelper.Action(actionContext);
            var newUrl = _urlLocalizer.LocalizeLink(url);

            return newUrl;
        }

        public override string Content(string contentPath)
        {
            return _urlHelper.Content(contentPath);
        }

        public override bool IsLocalUrl(string url)
        {
            return _urlHelper.IsLocalUrl(url);
        }

        public override string Link(string routeName, object values)
        {
            return _urlHelper.Link(routeName, values);
        }

        public override string RouteUrl(UrlRouteContext routeContext)
        {
            return _urlHelper.RouteUrl(routeContext);
        }
    }
}
