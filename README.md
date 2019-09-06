# ASP.NET Core Url Localization

[![nuget](https://img.shields.io/nuget/v/AspNetCore.Mvc.UrlLocalization.svg)](https://www.nuget.org/packages/AspNetCore.Mvc.UrlLocalization/) ![Downloads](https://img.shields.io/nuget/dt/AspNetCore.Mvc.UrlLocalization.svg "Downloads")

By default ASP.NET comes with support for [Globalization and Localization](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/localization?view=aspnetcore-2.2) but it is used only for 
* Formatting - (Dates, Currencies, Numbers) - CultureInfo.CurrentCulture 
* Resource Localization via Resx files - CultureInfo.CurrentUICulture

The aim of this package is to build on top of Resource Localization to provide 'Url Localization' via files named Url.{culture}.resx

## Installation

### NuGet
```
PM> Install-Package Hangfire.AspNetCore.UrlLocalization
```

### .Net CLI
```
> dotnet add package Hangfire.AspNetCore.UrlLocalization
```

## Examples
* See Examples\AspNetCore3
* See Examples\AspNetCore2.2
* See Examples\AspNetCore2.2FullFramework

## Authors

* **Dave Ikin** - [davidikin45](https://github.com/davidikin45)

## License

This project is licensed under the MIT License

## Acknowledgments

* [Url culture provider using middleware as filters in ASP.NET Core 1.1.0](https://andrewlock.net/url-culture-provider-using-middleware-as-mvc-filter-in-asp-net-core-1-1-0/)
* [Localized routes with ASP.NET 5 and MVC 6](https://www.strathweb.com/2015/11/localized-routes-with-asp-net-5-and-mvc-6/)
* [ASP.NET Core 2.1 MVC localized routing](https://github.com/saaratrix/asp.net-core-mvc-localized-routing)