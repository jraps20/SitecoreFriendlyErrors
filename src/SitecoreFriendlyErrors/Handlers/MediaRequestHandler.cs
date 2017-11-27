using System.Web;
using Sitecore;
using Sitecore.Configuration;
using Sitecore.Diagnostics;
using Sitecore.Resources.Media;
using Sitecore.SecurityModel;

namespace SitecoreFriendlyErrors.Handlers
{
    public class MediaRequestHandler : global::Sitecore.Resources.Media.MediaRequestHandler
    {
        protected override bool DoProcessRequest(HttpContext context)
        {
            Assert.ArgumentNotNull(context, "context");

            var request = MediaManager.ParseMediaRequest(context.Request);

            if (request == null)
                return false;

            var media = MediaManager.GetMedia(request.MediaUri);

            if (media != null)
                return DoProcessRequest(context, request, media);

            using (new SecurityDisabler())
                media = MediaManager.GetMedia(request.MediaUri);

            string str;

            if (media == null)
            {
                str = Settings.ItemNotFoundUrl;
            }
            else
            {
                Assert.IsNotNull(Context.Site, "site");
                str = Context.Site.LoginPage != string.Empty ? Context.Site.LoginPage : Settings.NoAccessUrl;
            }
            if (Settings.RequestErrors.UseServerSideRedirect)
                HttpContext.Current.Server.TransferRequest(str);
            else
                HttpContext.Current.Response.Redirect(str);
            return true;
        }
    }
}