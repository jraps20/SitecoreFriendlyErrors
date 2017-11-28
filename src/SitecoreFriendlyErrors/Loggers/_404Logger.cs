using log4net;
using Sitecore.Diagnostics;

namespace SitecoreFriendlyErrors.Loggers
{
    public static class _404Logger
    {
        public static ILog Log => LogManager.GetLogger("SitecoreFriendlyErrors.Loggers._404Logger") ?? LoggerFactory.GetLogger(typeof(_404Logger));
    }
}