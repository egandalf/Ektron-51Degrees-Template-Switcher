using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ektron.Cms.BusinessObjects;
using Ektron.Cms.Device;
using Ektron.Cms.Common;
using Ektron.Cms;

namespace eGandalf
{
    public class MobileDeviceModule : IHttpModule
    {

        void context_BeginRequest(object sender, EventArgs e)
        {
            /*
             * Most developers will employ a standard format for mobile vs. non-mobile templates 
             * when using device-based template switching. You can add that logic here.
             * 
             * In my case, I'm adding "mobile" to the end of the file name. 
             * So default.aspx becomes defaultmobile.aspx.
             */

            /* 
             * I'm using a limited set of the properties available from 51Degrees for 
             * this sample. For a complete listing, see: 
             * https://51degrees.com/resources/property-dictionary
             * 
             * Note that different packages return different properties, so pay attention 
             * to the levels beneath each property name.
             * 
             * This uses the Basic option, which uses data from the cloud.
             */

            /*
             * API source code and usage information can be found at 
             * https://github.com/51Degrees/dotNET-Device-Detection
             */

            string[] extensionsWhitelist = { ".aspx", ".html", ".js", ".ashx" };
            string[] foldersBlacklist = { "/workarea/", "/widgets/", "/ux/", "/assets/", "/privateassets/", "/uploadedimages/", "/uploadedfiles/", "/episerverfind/" };
            /*
             * To prevent extra processing, limit the files affected by the automatic redirect.
             */

            string oPath = HttpContext.Current.Request.Url.LocalPath;
            /*
             * Get the requested path (without alias).
             */

            bool extensionIsValid = false;
            bool folderIsValid = false;
            /*
             * Assume we're not going to redirect.
             */

            string lowerPath = oPath.ToLower();
            extensionIsValid = extensionsWhitelist.Any(lowerPath.EndsWith);
            /*
             * Use LINQ to make sure the extension is whitelisted.
             */

            folderIsValid = !foldersBlacklist.Any(lowerPath.StartsWith);
            /*
             * Use LINQ to make sure the path isn't blacklisted.
             */

            if (extensionIsValid && folderIsValid)
            {
                bool isMobile = false;
                /*
                 * I assume False, which will default any unknowns to the Desktop version
                 * of the site.
                 * 
                 * A reasonable alternative, given how frequently devices are released and
                 * updated, is to assume Mobile and rely on the data source to more accurately
                 * detect Desktops.
                 */

                bool success = bool.TryParse(HttpContext.Current.Request.Browser["IsMobile"], out isMobile);
                /*
                 * 51Degrees extends the Browser object in the request to add keyed properties
                 * for quick access. All I'm using here is the IsMobile determination. You could
                 * as easily add checks for IsTablet, for example.
                 */

                if (success && isMobile)
                {
                    /*
                     * Simple logic to insert "mobile" before the last period in the requested file.
                     */
                    string newPath = string.Empty;
                    int extIndex = oPath.LastIndexOf('.');
                    newPath = oPath.Insert(extIndex, "mobile");

                    /*
                     * Check that file exists before rewriting the URL. Allows desktop templates to load in
                     * cases where there is no Mobile equivalent.
                     */
                    if (System.IO.File.Exists(HttpContext.Current.Server.MapPath(newPath)))
                    {
                        HttpContext.Current.RewritePath(
                            HttpContext.Current.Request.Url.PathAndQuery.Replace(oPath, newPath),
                            false);
                        /*
                         * Rewrite serves the new template without changing the URL for the end user. As with the
                         * former built-in WURFL redirect, this ensures consistent URLs when shared via social or
                         * elsewhere.
                         */
                    }
                }
            }
        }

        #region IHttpModule Members

        public void Dispose()
        {

        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += new EventHandler(context_BeginRequest);
        }

        #endregion
    }
}