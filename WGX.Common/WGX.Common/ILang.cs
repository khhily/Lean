using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace WGX.Common {
    public interface ILang {

        ModelMetadataProvider GetMetadataProvider();

        string GetByKey(string key);
    }
}
