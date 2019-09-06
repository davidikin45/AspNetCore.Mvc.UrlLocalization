using Microsoft.Extensions.Localization;

namespace AspNetCore.Mvc.UrlLocalization
{
    public interface IUrlLocalizer : IStringLocalizer
    {
        string LocalizeLink(string path);

        string LocalizeRequestPath(string path);
        string UnlocalizeRequestPath(string path);
    }
}
