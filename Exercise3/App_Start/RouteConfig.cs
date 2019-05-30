using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Exercise3
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{action}/{id}",
                defaults: new { controller = "Map", action = "Default", id = UrlParameter.Optional }
            );

            //routes.MapRoute(
            //    name: "display", 
            //    url: "display/{ip}/{port}",
            //    defaults: new { controller = "Map", action = "Index" }
            //    );

            routes.MapRoute(
                name: "displayFour", 
                url: "display/{ip}/{port}/{time}",
                defaults: new { controller = "Map", action = "displayFourTimes" }
            );

            routes.MapRoute(
                name: "displayAndSave",
                url: "save/{ip}/{port}/{time}/{range}/{fileName}",
                defaults: new { controller = "Map", action = "displayAndSaveFunc" }
            );

            //routes.MapRoute(
            //    name: "load",
            //    url: "display/{fileName}/{time}",
            //    defaults: new { controller = "Map", action = "loadAndDisplay" }
            //);

            routes.MapRoute(
                name: "check",
                url: "display/{str}/{number}",
                defaults: new { controller = "Map", action = "checkFunc" }
            );
        }
    }
}
