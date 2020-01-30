using System;
using System.Net;
using System.Web;
using Sitecore.Configuration;
using Sitecore.Pipelines.HttpRequest;
using SitecoreFriendlyErrors.Loggers;

namespace SitecoreFriendlyErrors.Pipelines.HttpRequestEnd
{
    public class Set404StatusCode : HttpRequestBase
    {
        protected override void Execute(HttpRequestArgs args)
        {
            var httpContext = args.HttpContext;

            // retain 500 response if previously set
            if (HttpContext.Current.Response.StatusCode >= 500 || httpContext.Request.RawUrl == "/" || Sitecore.Context.Site == null)
                return;

            // return if request does not end with value set in ItemNotFoundUrl, i.e. successful page
            if (!httpContext.Request.Url.LocalPath.EndsWith(Settings.ItemNotFoundUrl, StringComparison.InvariantCultureIgnoreCase))
                return;

            _404Logger.Log.Warn($"Page Not Found on {Sitecore.Context.Site.Name}: {httpContext.Request.RawUrl}, current status: {HttpContext.Current.Response.StatusCode}");
            HttpContext.Current.Response.TrySkipIisCustomErrors = true;
            HttpContext.Current.Response.StatusCode = (int)HttpStatusCode.NotFound;
            HttpContext.Current.Response.StatusDescription = "Page not found";
        }
    }
}