# SitecoreFriendlyErrors

Quickly get custom/friendly error pages configured in Sitecore. 

### Disclaimer

_Verified on Sitecore 8.2+, including Sitecore 9. If you use this module with success, please let me know and I will update supported versions. To the best of my knowledge, the depedent logic has not been updated for quite some time._

_Use at your own risk. While all changes are documented and available here (and this technique is used on multiple websites in production), these updates replace two pieces of existing Sitecore functionality:_
* `<processor type="Sitecore.Pipelines.HttpRequest.ExecuteRequest, Sitecore.Kernel"/>`
* `<add verb="*" path="sitecore_media.ashx" type="Sitecore.Resources.Media.MediaRequestHandler, Sitecore.Kernel" name="Sitecore.MediaRequestHandler" />`

_The two replacement classes inherit the originals. These pieces are intentionally replaced to alter how the `RequestErrors.UseServerSideRedirect` setting is interpretted. By default, when true, Sitecore runs `Server.Transfer` on the given URL. `Server.Transfer` does not re-execute the request, instead it sends the user directly to a page, which is why the default `ItemNotFoundUrl` is a physical page on the server, it bypasses all pipelines associated with the request. The code in this repository changes this to use `Server.TransferRequest`, which sends the request behind the scenes and re-executes the request with all pipelines. This allows us to pass a Sitecore website path as the 404 page._

# Installation

1. Install the NuGet Package in your Sitecore web project: https://www.nuget.org/packages/Sitecore.FriendlyErrors
2. Build and publish web project to web root
3. Add an item named "404" beneath all websites _ex: /sitecore/content/home/404_ __Be sure this item has a layout defined in Presentation Details__
4. Verify friendly 404 page is returned by visiting an incorrect link on your website
5. Review Next Steps below

If working properly, your URL should remain as entered, however the response from the server should be a __404__. You can verify with Fiddler or Inspect Element > Network traffic in Chrome.

# Next Steps

By default, all error modes are set to RemoteOnly (or the equivalent). These are the proper settings for a production environment. To test the static 404 and 500 pages make the following changes to the web.config:

1. `<configuration> <system.web> <customErrors mode="On"...`
2. `<configuration> <system.webServer> <httpErrors errorMode="Custom"...`

With these changes in place, you can test the static 404 page by appending a false extension to a URL. For example, http://mysite.local/page.fake. It should return the static 404 page and a 404 status code to the browser.

To test the custom 500 page, the simplest method is to break a connection string in `App_Config\ConnectionStrings.config`. Edit the username to be incorrect. Any request should then return the custom 500.aspx page, with a 500 status code returned to the browser.

## Update 400.html and 500.aspx to be on brand

The package installs `~\400.html` and `~\500.aspx`. Update these files to include the site CSS and brand as needed. You may also wish to add additional logic to the 500 page to be specific to each site (by default there is one 500 error used for all sites).

# Modifications Made By Package

### Files Added
* \bin\SitecoreFriendlyErrors.dll
* \App_Config\Include\Z.SitecoreFriendlyErrors.config
* \404.html
* \500.aspx

### Files Modified (Based on Vanilla Sitecore install)
* \web.config
   * `system.web\customErrors`
   * `system.webServer\handlers\Sitecore.MediaRequestHandler`
   * `system.webServer\httpErrors`
   
_All modifications are reverted if the package is uninstalled. Original Sitecore values will be reverted._

# Guide

This package is based off my thorough posts on Sitecore Friendly Errors...

* https://www.geekhive.com/buzz/post/2017/07/a-complete-guide-to-configuring-friendly-error-pages-in-sitecore-part-1-404-pages/
* https://www.geekhive.com/buzz/post/2017/08/a-complete-guide-to-configuring-friendly-error-pages-in-sitecore-part-2-500-pages/