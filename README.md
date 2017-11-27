# SitecoreFriendlyErrors

Quickly get custom/friendly error pages configured in Sitecore. 

# Installation

1. Install the NuGet Package in your Sitecore web project: https://www.nuget.org/packages/Sitecore.FriendlyErrors
2. Build and publish web project to web root
3. Add an item named "404" beneath all websites _ex: /sitecore/content/home/404_ __Be sure this item has a layout defined in Presentation Details__
4. Verify friendly 404 page is returned by visiting an incorrect link on your website

If working properly, your URL should remain as entered, however the response from the server should be a __404__. You can verify with Fiddler or Inspect Element > Network traffic in Chrome.

# Further Testing

By default, all error modes are set to RemoteOnly (or the equivalent). To test with all error modes on, make the following changes to the web.config:

1. `<configuration> <system.web> <customErrors mode="On"...`
2. `<configuration> <system.webServer> <httpErrors errorMode="Custom"...`

With these changes in place, you can test the static 404 page by appending a false extension to a URL. For example, http://mysite.local/page.fake. It should return the static 404 page and a 404 status code to the browser.

To test the custom 500 page, the simplest method is to break a connection string in `App_Config\ConnectionStrings.config`. Edit the username to be incorrect. Any request should then return the custom 500.aspx page, with a 500 status code returned to the browser.

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