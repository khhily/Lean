using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using WGX.Common;

namespace Lean
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.Add(new Route("{lang}/{controller}/{action}/{id}",
                            new RouteValueDictionary(new
                            {
                                lang = "zh-CN",
                                controller = "Home",
                                action = "Index",
                                id = UrlParameter.Optional
                            }),
                            new RouteValueDictionary(new
                            {
                                lang = "(zh-CN)|(en-US)"
                            }),
                            new MutiLangRouteHandler()));

            routes.Add(new Route("{controller}/{action}/{id}",
                            new RouteValueDictionary(new
                            {
                                lang = "zh-CN",
                                controller = "Home",
                                action = "Index",
                                id = UrlParameter.Optional
                            }), new MutiLangRouteHandler()));

            //routes.MapRoute(
            //    name: "Default",
            //    url: "{controller}/{action}/{id}",
            //    defaults: new { lang = "en-US", controller = "Home", action = "Index", id = UrlParameter.Optional }
            //);
        }
    }
}
