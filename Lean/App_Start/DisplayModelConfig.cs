using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.WebPages;
using WGX.Common.Enums;
using WGX.Common.Helper;

namespace Lean
{
    public static class DisplayModelConfig
    {

        public static void Config()
        {

            DisplayModeProvider.Instance.Modes.Insert(0, new DefaultDisplayMode("en-US")
            {
                ContextCondition = ctx =>
                {
                    //var lang = SessionHelper.Get<string>(SessionKeys.Lang);
                    var data = RouteTable.Routes.GetRouteData(ctx);
                    if (data != null)
                    {
                        var lang = (string)data.Values.Get("lang", ""); //["lang"];
                        if (string.IsNullOrEmpty(lang))
                        {
                            data.Values.Add("lang", "en-US");
                            lang = "en-US";
                        }
                        return string.Equals(lang, "en-US", StringComparison.OrdinalIgnoreCase);
                    }
                    return false;
                }
            });
        }


    }
}