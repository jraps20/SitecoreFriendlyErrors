using System.Web;
using Microsoft.Extensions.DependencyInjection;
using Sitecore;
using Sitecore.Abstractions;
using Sitecore.Configuration;
using Sitecore.DependencyInjection;
using Sitecore.Web;
using SitecoreFriendlyErrors.Loggers;

namespace SitecoreFriendlyErrors.Pipelines.HttpRequestBegin
{
    public class ExecuteRequest : global::Sitecore.Pipelines.HttpRequest.ExecuteRequest
    {
        private readonly BaseLinkManager _baseLinkManager;
        
        public ExecuteRequest(BaseSiteManager siteManager, BaseItemManager itemManager) : base(siteManager, itemManager)
        {
            _baseLinkManager = ServiceLocator.ServiceProvider.GetRequiredService<BaseLinkManager>();
        }
        
        protected override void PerformRedirect(string url)
        {
            if (Context.Site == null || Context.Database == null || Context.Database.Name == "core")
            {
                _404Logger.Log.Info($"Attempting to redirect url {url}, but no Context Site or DB defined (or core db redirect attempted)");
                return;
            }

            // need to retrieve not found item to account for sites utilizing virtualFolder attribute
            var notFoundItem = Context.Database.GetItem(Context.Site.StartPath + Settings.ItemNotFoundUrl);

            if (notFoundItem == null)
            {
                _404Logger.Log.Info($"No 404 item found on site: {Context.Site.Name}");
                return;
            }

            var notFoundUrl = _baseLinkManager.GetItemUrl(notFoundItem);

            if (string.IsNullOrWhiteSpace(notFoundUrl))
            {
                _404Logger.Log.Info($"Found 404 item for site, but no URL returned: {Context.Site.Name}");
                return;
            }

            _404Logger.Log.Info($"Redirecting to {notFoundUrl}");
            if (Settings.RequestErrors.UseServerSideRedirect)
            {
                HttpContext.Current.Server.TransferRequest(notFoundUrl);
            }
            else
                WebUtil.Redirect(notFoundUrl, false);
        }
    }
}