using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WGX.Common {
    /// <summary>
    /// 
    /// </summary>
    public interface IEnumDescriptionProvider {
        string GetDescription(string key);
    }
}
