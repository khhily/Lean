using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;

namespace WGX.Common {
    public class MutiLangRouteHandler : MvcRouteHandler {

        protected override System.Web.IHttpHandler GetHttpHandler(RequestContext requestContext) {
            var handler = base.GetHttpHandler(requestContext);
            string lang = requestContext.RouteData.Values["lang"].ToString();
            try {
                var culture = CultureInfo.GetCultureInfo(lang);
                Thread.CurrentThread.CurrentUICulture = culture;
            } catch {

            }

            return handler;
        }

    }
}
