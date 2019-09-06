using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System.Collections.Generic;
using System.Linq;

namespace AspNetCore.Mvc.UrlLocalization
{
    //https://andrewlock.net/url-culture-provider-using-middleware-as-mvc-filter-in-asp-net-core-1-1-0/
    //https://andrewlock.net/applying-the-routedatarequest-cultureprovider-globally-with-middleware-as-filters/
    //https://andrewlock.net/using-a-culture-constraint-and-catching-404s-with-the-url-culture-provider/
    //https://andrewlock.net/redirecting-unknown-cultures-to-the-default-culture-when-using-the-url-culture-provider/
    public class UrlLocalizationConvention : IApplicationModelConvention, IControllerModelConvention, IActionModelConvention
    {
        private readonly bool _optional;
        private readonly string _cultureRouteDataStringKey;
        private readonly string _cultureConstraintKey;
        private readonly AttributeRouteModel _culturePrefix;

        public UrlLocalizationConvention(bool optional, string cultureRouteDataStringKey, string cultureConstraintKey)
        {
            _optional = optional;
            _cultureRouteDataStringKey = cultureRouteDataStringKey;
            _cultureConstraintKey = cultureConstraintKey;
            _culturePrefix = new AttributeRouteModel(new RouteAttribute("{" + _cultureRouteDataStringKey + (!string.IsNullOrEmpty(_cultureConstraintKey) ? $":{_cultureConstraintKey}" : "") + "}"));
        }

        public void Apply(ApplicationModel application)
        {
            foreach (var controller in application.Controllers)
            {
                Apply(controller);
            }
        }

        public void Apply(ControllerModel controller)
        {
            var newSelectors = new List<SelectorModel>();

            //Controller has Attribute Route
            var matchedSelectors = controller.Selectors.Where(x => x.AttributeRouteModel != null).ToList();
            if (matchedSelectors.Any())
            {
                foreach (var selectorModel in matchedSelectors)
                {
                    var routeModel = AttributeRouteModel.CombineAttributeRouteModel(_culturePrefix, selectorModel.AttributeRouteModel);

                    if (_optional)
                    {
                        var newSelector = new SelectorModel();
                        newSelector.AttributeRouteModel = routeModel;
                        newSelector.AttributeRouteModel.Order = -1;
                        newSelectors.Add(newSelector);
                    }
                    else
                    {
                        selectorModel.AttributeRouteModel = routeModel;
                    }
                }
            }

            foreach (var action in controller.Actions)
            {
                Apply(action);
            }

            foreach (var newSelector in newSelectors)
            {
                controller.Selectors.Insert(0, newSelector);
            }
        }

        public void Apply(ActionModel action)
        {
            var newSelectors = new List<SelectorModel>();

            //Action has Attribute Route
            var matchedSelectors = action.Selectors.Where(x => x.AttributeRouteModel != null).ToList();
            if (matchedSelectors.Any())
            {
                foreach (var selectorModel in matchedSelectors)
                {
                    //https://andrewlock.net/applying-the-routedatarequest-cultureprovider-globally-with-middleware-as-filters/
                    if (selectorModel.AttributeRouteModel.Template != null && selectorModel.AttributeRouteModel.Template.StartsWith("~/"))
                    {
                        var path = selectorModel.AttributeRouteModel.Template.Length > 2 ? $"/{selectorModel.AttributeRouteModel.Template.Substring(2)}" : "";
                        var routeModel = new AttributeRouteModel(new RouteAttribute($"~/{_culturePrefix.Template}{path}"));

                        if (_optional)
                        {
                            var newSelector = new SelectorModel();
                            newSelector.AttributeRouteModel = routeModel;
                            newSelector.AttributeRouteModel.Order = -1;
                            newSelectors.Add(newSelector);
                        }
                        else{
                            selectorModel.AttributeRouteModel = routeModel;
                        }
                    }
                    //https://andrewlock.net/applying-the-routedatarequest-cultureprovider-globally-with-middleware-as-filters/
                    else if (selectorModel.AttributeRouteModel.Template != null && selectorModel.AttributeRouteModel.Template.StartsWith("/"))
                    {
                        var path = selectorModel.AttributeRouteModel.Template.Length > 1 ? $"/{selectorModel.AttributeRouteModel.Template.Substring(1)}" : "";
                        var routeModel = new AttributeRouteModel(new RouteAttribute($"/{_culturePrefix.Template}{path}"));

                        if (_optional)
                        {
                            var newSelector = new SelectorModel();
                            newSelector.AttributeRouteModel = routeModel;
                            newSelector.AttributeRouteModel.Order = -1;
                            newSelectors.Add(newSelector);
                        }
                        else
                        {
                            selectorModel.AttributeRouteModel = routeModel;
                        }
                    }
                    //Controller doesn't have Attribute Route
                    else if (action.Controller.Selectors.Where(x => x.AttributeRouteModel == null).ToList().Any())
                    {
                        var routeModel = AttributeRouteModel.CombineAttributeRouteModel(_culturePrefix, selectorModel.AttributeRouteModel);

                        if (_optional)
                        {
                            var newSelector = new SelectorModel();
                            newSelector.AttributeRouteModel = routeModel;
                            newSelector.AttributeRouteModel.Order = -1;
                            newSelectors.Add(newSelector);
                        }
                        else
                        {
                            selectorModel.AttributeRouteModel = routeModel;
                        }
                    }
                }
            }

            foreach (var newSelector in newSelectors)
            {
                action.Selectors.Insert(0, newSelector);
            }
        }
    }
}
