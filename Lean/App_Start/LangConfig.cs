using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using WGX.Common;

namespace Lean {
    public class LangConfig {

        public static void Config() {
            ILang lang = DependencyResolver.Current.GetService<ILang>();
            ModelMetadataProviders.Current = lang.GetMetadataProvider();

            WGX.Common.Helper.EnumHelper.Init(DependencyResolver.Current.GetService<IEnumDescriptionProvider>());
        }
    }
}
