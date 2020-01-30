using System;
using System.Linq;
using Sitecore;
using Sitecore.Pipelines.HttpRequest;

namespace SitecoreFriendlyErrors.Pipelines
{
    public abstract class HttpRequestBase : HttpRequestProcessor
    {
        /// <summary>
        /// allowedSites and disallowedSites are mutually exclusive, use one or the other
        /// in the event both are used, disallowedSites is enforced
        /// </summary>
        public string allowedSites { get; set; }
        public string disallowedSites { get; set; }
        public string ignoredPaths { get; set; }
        public string ignoredModes { get; set; }
        /// <summary>
        /// allowedDatabases and disallowedDatabases are mutually exclusive, use one or the other
        /// in the event both are used, disallowedDatabases is enforced
        /// </summary>
        public string allowedDatabases { get; set; }
        public string disallowedDatabases { get; set; }
        
        /// <summary>
        /// Overridden HttpRequestProcessor method
        /// </summary>
        /// <param name="args"></param>
        public override void Process(HttpRequestArgs args)
        {
            if (IsValid(args))
            {
                Execute(args);
            }
        }

        protected abstract void Execute(HttpRequestArgs args);

        protected virtual bool IsValid(HttpRequestArgs hArgs)
        {
            return SitesAllowed()
                && PathNotIgnored(hArgs)
                && ModeNotIgnored()
                && DatabaseAllowed();
        }

        private bool SitesAllowed()
        {
            // httpRequest processors should never run without a context site
            if (Context.Site == null)
                return false;

            var contextSiteName = Context.GetSiteName();

            if (string.IsNullOrWhiteSpace(contextSiteName))
                return false;

            // disallow checked first to trump an allowance
            if (!string.IsNullOrWhiteSpace(disallowedSites))
            {
                return !disallowedSites
                    .Split(',')
                    .Select(i => i.Trim())
                    .Any(siteName => string.Equals(siteName, contextSiteName, StringComparison.CurrentCultureIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(allowedSites))
            {
                return allowedSites
                    .Split(',')
                    .Select(i => i.Trim())
                    .Any(siteName => string.Equals(siteName, contextSiteName, StringComparison.CurrentCultureIgnoreCase));
            }

            return true;
        }

        private bool PathNotIgnored(HttpRequestArgs hArgs)
        {
            if (string.IsNullOrWhiteSpace(ignoredPaths))
                return true;
            
            var httpContext = hArgs.HttpContext;

            var ignoredPath = ignoredPaths
                .Split(',')
                .Select(i => i.Trim())
                .Any(path => httpContext.Request.RawUrl.StartsWith(path, StringComparison.CurrentCultureIgnoreCase));

            return !ignoredPath;
        }

        private bool ModeNotIgnored()
        {
            if (string.IsNullOrWhiteSpace(ignoredModes))
                return true;

            var modes = ignoredModes.Split(',').Select(i => i.Trim());

            var isEditor = Context.PageMode.IsExperienceEditor;

            return !modes.Any(mode =>
              (mode == "Edit" && isEditor) ||
              (mode == "Preview" && Context.PageMode.IsPreview)
            );
        }

        private bool DatabaseAllowed()
        {
            // httpRequest processors should never run without a context database
            if (Context.Database == null)
                return false;

            var contextDatabaseName = Context.Database.Name;

            // disallow checked first to trump an allowance
            if (!string.IsNullOrWhiteSpace(disallowedDatabases))
            {
                return !disallowedDatabases
                    .Split(',')
                    .Select(i => i.Trim())
                    .Any(database => string.Equals(database, contextDatabaseName, StringComparison.CurrentCultureIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(allowedDatabases))
            {
                return allowedDatabases
                    .Split(',')
                    .Select(i => i.Trim())
                    .Any(database => string.Equals(database, contextDatabaseName, StringComparison.CurrentCultureIgnoreCase));
            }

            return true;
        }
    }
}