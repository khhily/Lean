using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using WGX.Common;
using WGX.Common.Helper;
using WGX.Lean.Res;

namespace Lean {
    public class Lang : ILang, IEnumDescriptionProvider {
        public ModelMetadataProvider GetMetadataProvider() {
            return new ResDataAnnotationsModelMetadataProvider(EntityRes.ResourceManager);
        }


        public string GetByKey(string key) {
            return EntityRes.ResourceManager.GetString(key);
        }

        public string GetDescription(string key) {
            return EnumRes.ResourceManager.GetString(key);
        }
    }
}
